using CV19.Infrastructure.Commands;
using CV19.Models;
using CV19.Models.Decanat;
using CV19.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;


namespace CV19.ViewModels
{
    // : ViewModel наследуем всю функциональность от этого класса, это события и 2 основных метода. При желании можно переопределить, освободить ресурсы, которые модель захватит вдруг
    internal class MainWindowViewModel : ViewModel
    {
        /*------------------------------------------------------------------------------------------------------------------------ */

        public ObservableCollection<Group> Groups { get; }
        public object[] CompositeCollection { get; }

        #region SelectedCompositeValue : object - Выбранный непонятный элемент
        /// <summary>Выбранный непонятный элемент</summary>
        private object _SelectedCompositeValue;
        /// <summary>Выбранный непонятный элемент</summary>
        public object SelectedCompositeValue { 
            get => _SelectedCompositeValue;
            set => Set(ref _SelectedCompositeValue, value);
        }

        #endregion

        #region SelectedGroup : Group - выбранная группа
        //Теперь мы можем указать визуальному списку (DataGrid) тепрь, что его св-во SelectedItem теперь будет связано со св-ом SelectedGroup
        //и наша ViewModels будет ощущать, что мы перемещаемся между элементами списка, сюда (в SelectedGroup) будет попадать все время новая группа
        //и в set => Set(ref _SelectedGroup, value); , можно определить логику, которая необходима для обработки в интерфейсе выбираемой группы

        /// <summary>Выбранная группа </summary>
        private Group _SelectedGroup;


        /// <summary> Выбранная группа </summary>
        public Group SelectedGroup 
        { 
            get => _SelectedGroup; 
            set => Set(ref _SelectedGroup, value); 
        }
        #endregion


        /*ViewModel, её основная задача - содержать в себе набор свойств, которые привязаны к визуальным элементам в дизайнере
            и вся логика ViewModel, изменять значение этих свойств внутри кода , а элементы интерфейса будут обнаруживать эти изменения
            и перерисовываться соответствующим образом.*/



        #region TestDataPoints : IEnumerable<DataPoint> - Тестовый набор данных для визуализации графиков
        //НАм понадобиться сво-во для перечесления точек данных, которые мы будем строить на графике
        /// <summary>
        /// Тестовый набор данных для визуализации графиков
        /// </summary>
        private IEnumerable<DataPoint> _TestDataPoints;
        /// <summary>
        /// Тестовый набор данных для визуализации графиков
        /// </summary>
        public IEnumerable<DataPoint> TestDataPoints 
        { 
            get => _TestDataPoints; 
            set => Set(ref _TestDataPoints, value); 
        }

        #endregion


        #region Заголовок окна
        // Проверяем, что ViewModel реально работает, для этого надо создать какое-то свойство и подцепить к ниму визуальные элементы,чтобы они из него получили данные.
        // //Для этого обычно создаю заголовок окна и св-во окна, которое должно отображаться внизу в статус-баре
        private string _Title = "Анализ статистики CV19";
        /// <summary>
        /// Заголовок окна
        /// </summary>
        public string Title
        {
            get => _Title;
            /*set //Так развернуто будет выглядить под капотом мутод Set
            {
                *//*if (Equals(_Title, value)) return;
                _Title = value;
                OnPropertyChanged();*//*
                Set(ref _Title, value);
            }*/
            set => Set(ref _Title, value);
        }
        #endregion

        #region Status : string - Статус программы
        /// <summary>
        /// Статус программы
        /// </summary>
        /// Создается пара, поле (в xaml) и свойство к этому полю. Геттер возвращает значение поля, сеттер вызывает метод Set, в который передаем ссылку на это поле и значение, которое он будет устанавливать. 
        private string _Status = "Готов!";
        /// <summary>
        /// Статус программы
        /// </summary>
        public string Status 
        {   
            get => _Status; 
            set => Set(ref _Status, value); 
        }

        #endregion

        /*------------------------------------------------------------------------------------------------------------------------ */

        #region Команды
        //Команда, которая позволит закрывать нашу программу
        #region CloseApplicationCommand

        public ICommand CloseApplicationCommand { get;}

        //OnCloseApplicationCommandExecuted - этот метод будет выполнятся, когда команда выполняется.
        private void OnCloseApplicationCommandExecuted(object p)
        {
            Application.Current.Shutdown();
        }

        private bool CanCloseApplicationCommandExecute(object p) => true; //Команда будет доступна для выполнения всегда, поэтому TRUE
        #endregion

        public ICommand CreateGroupCommand { get; }

        private bool CanCreateGroupCommandExecute(object p) => true;
        private void OnCreateGroupCommandExecut(object p)
        {
            var student_index = 1;
            //Создаем перечесление студентов в группе, скажем 10 человек на одну группу. Каждому присваиваем класс студент
            var students = Enumerable.Range(1, 10).Select(i => new Student
            {
                Name = $"{student_index}",
                Surname = $"Surname{student_index}",
                Patronymic = $"Patronymic{student_index++}",
                Birthday = DateTime.Now,
                Rating = 0
            });
            var group_max_index = Groups.Count + 1;
            var new_group = new Group
            {
                Name = $"Группа {group_max_index}",
                Students = new ObservableCollection<Student>(students)
            };
            Groups.Add(new_group);
        }

        #region DeleteGroupCommand
        public ICommand DeleteGroupCommand { get; }
        private bool CanDeleteGroupCommandExecute(object p) => p is Group group && Groups.Contains(group);
        private void OnDeleteGroupCommandExecut(object p)
        {
            if (!(p is Group group)) return;
            var group_index = Groups.IndexOf(group);
            Groups.Remove(group);
            if (group_index < Groups.Count)
                SelectedGroup = Groups[group_index];
        }
        #endregion

        #endregion

        //Теперь создаем команды внутри конструктора

        /*------------------------------------------------------------------------------------------------------------------------ */

        public MainWindowViewModel()
        {
            #region Команды

            CloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecute);
            CreateGroupCommand = new LambdaCommand(OnCreateGroupCommandExecut, CanCreateGroupCommandExecute);
            DeleteGroupCommand = new LambdaCommand(OnDeleteGroupCommandExecut, CanDeleteGroupCommandExecute);

            #endregion

            var data_points = new List<DataPoint>((int)(360 / 0.1));
            for(var x = 0d;x <= 360; x += 0.1)
            {
                const double to_rad = Math.PI / 180;
                var y = Math.Sin(x * to_rad);

                data_points.Add(new DataPoint { XValue = x, YValue = y});
            }
            TestDataPoints = data_points;

            var student_index = 1;

            //Создаем перечесление студентов в группе, скажем 10 человек на одну группу. Каждому присваиваем класс студент
            var students = Enumerable.Range(1, 10).Select(i => new Student
            {
                Name = $"{ student_index}",
                Surname = $"Surname{student_index}",
                Patronymic = $"Patronymic{student_index++}",
                Birthday = DateTime.Now,
                Rating = 0
            });

            //Создадим перечесление в кол-ве 20-ти штук, затем берем каждое число и на его основе создаем группу
            var groups = Enumerable.Range(1, 20).Select(i => new Group
            {
                Name = $"Группа {i}",
                Students = new ObservableCollection<Student>(students) //В каждую группу будет входит по 10 студентов
            }) ;

            Groups = new ObservableCollection<Group>(groups); //Теперь группы создаться гораздо быстрее в ObservableCollection

            var data_list = new List<object>();
            data_list.Add("Hello world");
            data_list.Add(42);
            var group = Groups[1];
            data_list.Add(group);
            data_list.Add(group.Students[0]);

            CompositeCollection = data_list.ToArray();

        }
    }
}



