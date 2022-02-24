using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace CV19.Models
{
    internal class PlaceInfo
    {
        public string Name { get; set; }   //Название страны
        public Point Location { get; set; }   //Местоположение её, которое можем извлечь. Point, обладающий структурой Х и У вещественного типа, как раз нам подходит

        public IEnumerable<ConfirmedCount> Counts { get; set; } // Инфа по кол-ву подтвержденных случаев
    }



}
