using System.Collections.Generic;

namespace CV19.Models
{
    //создаем двух наследников
    internal class CountryInfo : PlaceInfo  //Инфа по стране
    {
        public IEnumerable<ProvinceInfo> ProvinceCounts { get; set; }
    }



}
