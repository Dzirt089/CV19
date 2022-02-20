using CV19.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace CV19.ViewModels
{
    // : ViewModel наследуем всю функциональность от этого класса, это события и 2 основных метода. При желании можно переопределить, освободить ресурсы, которые модель захватит вдруг
    internal class MainWindowViewModel : ViewModel 
    {
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
            /*set
            {
                *//*if (Equals(_Title, value)) return;
                _Title = value;
                OnPropertyChanged();*//*
                Set(ref _Title, value);
            }*/
            set => Set(ref _Title, value);
        } 
        #endregion
    }
}
