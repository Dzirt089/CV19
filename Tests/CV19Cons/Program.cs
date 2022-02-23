using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Globalization;

namespace CV19Cons
{
    internal class Program
    {
        const string data_url = @"https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_covid19_confirmed_global.csv";

        //Первая часть, метод Task<Stream> GetDataStrim() будет возвращать поток, из которого можно будет читать текстовые данные
        //Задача у него такая, когда мы отправляем запрос на сервер и он нам отвечает, если файл огромный нам ненужно его скачивать полностью.
        //А только те данные, которые нам нужны. И если нам понадобиться, мы могли бы прервать извлечение данных из сети и при этом чтобы наша память не засорилась
        
        
        private static async Task<Stream> GetDataStrim() //Создаем ассинхронный метод, который нам вернёт поток (Stream)
        {
            var client = new HttpClient();    //Создает клиента внутри себя
            var response = await client.GetAsync(data_url, HttpCompletionOption.ResponseHeadersRead);    //Получает ответ от удаленного сервера.
         //HttpCompletionOption.ResponseHeadersRead - этим берем из содержимого ответа, только заголовки. Всё остальное остается в сетевой карте и сетевая карта остальные даннные пока не трогает

            return await response.Content.ReadAsStreamAsync(); //Возвращаем поток, который собственно и обеспечивает процесс чтения данных из сетевой карты
            //response - берем ответ, который получили от карты.Content - берем контент. И возвращаем ReadAsStreamAsync()
        }

       
        
        //Теперь будем читать текстовые данные и читая, разобьем их на строки. Т.е. чтобы каждая строка извлекалась отдельно.
        //Метод IEnumerable<string> GetDataLines() будет возвращать перечисление строк
        private static  IEnumerable<string> GetDataLines()  //Возвращаем интерфейс IEnumerable<string> строк
        {
            using var data_stream =  GetDataStrim().Result; //Получаем внутри метода поток
            using var data_reader = new StreamReader(data_stream);// На основе data_stream, создаем data_reader - который будет читать строковые данные. Скармливаем этому объекту поток.

            while (!data_reader.EndOfStream) //Читаем данные до тех пор, пока не встретиться конец потока.
            {
                var line = data_reader.ReadLine(); //Пока не конец потока, извлекаем из data_reader очередную строку и помещаем её в переменную
                if (string.IsNullOrWhiteSpace(line)) continue; //Проверяем, не пуста ли строка.Если пуста, то делаем следующий цикл
                yield return line.Replace("Korea,","Korea -").Replace("Bonaire,", "Bonaire -"); //Этот метод необычный, он будет генератором благодаря yield return. Он считывает строчку и возвращает как результат.
                //line.Replace("Korea,","Korea -") выполним замену запятой. Чтобы логика не нарушалась в дальнейшем
            }
        }

        //Мы используем интерфейс для using System.Linq;
        //Т.е.мы будем обрабатывать .csv файл с помощью команд языка Linq. Тем самым создаем конвеер, который будет обрабатывать данные (Распарсить например)
        //У нас первый метод формирует поток байт данных. 
        //Второй метод-генератор разбивает этот поток на последовательность строк 


        //Этот, третий метод, должен будт выделить необхдимые нам данные (например, распарсить первую строку которую мы получаем)
        // Извлечем из неё все даты, чтобы был массив дат по которому будем работать

        
        //Возвращаем массив DateTime[] => вызывем метод GetDataLines(), получив перечесление строк всего запросов.
        private static DateTime[] GetDates() => GetDataLines()
            .First()  // После этого говорим, что нас интересует только первая строка
            .Split(',')  // После этого берем первую строку и разбиваем строку.Вызываем Split по разделетелю (','). Получаем массив строк, который содержит заголовок каждой колонки csv-файла
            .Skip(4)  //После этого нам необходимо отбросить первые 4 (там находятся названия провинций и т.д.)
            .Select(s => DateTime.Parse(s, CultureInfo.InvariantCulture)) //Оставшиеся строки преобразуем в DateTime. Говорим, что у нас есть строка s и нам нужно её преобразовать путем её разбора.
            .ToArray();                               //Показываем что разбор строки s Parse(s, ). И указываем культуру которая понадобиться вторым параметром (s, CultureInfo.InvariantCulture)
                                                      //ToArray();  как результат. Т.е. такой запрос позволит нам получить все даты в массив




        //Теперь извлекаем данные по кол-ву зараженных по всем странам и каждой провинции этой страны
        //Основная идея метода интерфейса перечисления  (который будет возвращать также перечисления) заключается в том, что можно создать ленивый метод, который не делает всю работу если она не нужна (например взять первые 10 стран из запроса, остальные будет отброшено)
        //Внутри IEnumerable<> используем кортежи. Кортеж позволяет на лету в нужном мне месте определить структуру данных с нужным набором св-в, при этом кортеж отличается от анонимного класса тем, что это структура. Она создается на стыке вызова, не требует работы сборщика мусора и т.д.).
        // В () определяем структуру кортежа (страна, провинция, кол-во зараженных в каждый момент времени)
        private static IEnumerable<(string Contry,string Province, int[] Counts)> GetData()
        {
            var lines = GetDataLines()
                .Skip(1)    //Извлекаем пока общие данные, перечесление всех строк, которые мы можем извлечь по файлу. Первая строка, это заголовок, поэтому надо е отбросить
                .Select(line => line.Split(',')); //Каждую строку надо разбить по разделетилю запятой. Получаем перечесление массива строк, где каждый элемент это колонка (ячейка таблицы по сути) в строковом виде
                 //И их нужно теперь преобразовать в нужный нам кортеж

            foreach(var row in lines)   // Выделем сперва все данные в переменную, потом сгруппируем в кортеж и вернём его, чтобы было проще
            {
                var province = row[0].Trim();   //У каждой строки будем вызывать метод Trim(), который будет обрезать все лишнее в нашей строке (в плане пробелов, спец символов нечитаемых и т.д.)
                var country_name = row[1].Trim(' ','"'); //А вот для contry_name надо будет указать что конкретно мы хотим обрезать (пробелы и ковычки). ЗАпятаю не получится обрезать, это разделитель колонок и будут проблемы
                var i = 0;
                if (!int.TryParse(row[4], out int res))
                    i = 1;
                var counts = row.Skip(4 + i).Select(int.Parse).ToArray();
                //var counts = row.Skip(4).Select(s => int.Parse(s)).ToArray(); //Так как 2 и 3 столбцом идут широта и долгота, мы пропускаем их. Остальное - это кол-во зараженных на дату
               //Мы считали в каждую переменную данные по строчно. После чего, каждый из элементов мы превращаем в целое число


                yield return (province, country_name, counts); //С помощью yield return возвращаем данные в виде кортежа. 
            }

        } 

       
        
        static void Main(string[] args)
        {
            //WebClient client = new WebClient();

            //var client = new HttpClient();
            //var response = client.GetAsync(data_url).Result;
            //var csv_str = response.Content.ReadAsStringAsync().Result;

            //foreach(var data_line in GetDataLines()) //Используем наш метод-генератор IEnumerable<string> GetDataLines() 
            //{
            //    Console.WriteLine(data_line);
            //}

            //var dates = GetDates();
            //Console.WriteLine(string.Join("\r\n",dates));


            //Теперь можем делать запросы к этим данным в следующем виде
            var russia_data = GetData()
                .FirstOrDefault(v => v.Contry.Equals("Russia",StringComparison.OrdinalIgnoreCase));

            Console.WriteLine(string.Join("\r\n", GetDates().Zip(russia_data.Counts, (date,count) => $"{date:dd:MM} - {count}")));

            Console.ReadLine();
        }
    }
}
