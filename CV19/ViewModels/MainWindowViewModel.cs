using CV19.Infrastructure.Commands;
using CV19.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace CV19.ViewModels
{
    // : ViewModel наследуем всю функциональность от этого класса, это события и 2 основных метода. При желании можно переопределить, освободить ресурсы, которые модель захватит вдруг
    internal class MainWindowViewModel : ViewModel
    {
        /*ViewModel, её основная задача - содержать в себе набор свойств, которые привязаны к визуальным элементам в дизайнере
            и вся логика ViewModel, изменять значение этих свойств внутри кода , а элементы интерфейса будут обнаруживать эти изменения
            и перерисовываться соответствующим образом.*/
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
        #endregion

        //Теперь создаем команды внутри конструктора
        public MainWindowViewModel()
        {
            #region Команды

            CloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecute);

            #endregion
        }
    }
}



