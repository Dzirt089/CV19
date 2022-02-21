using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace CV19.Infrastructure.Commands.Base
{
    internal abstract class Command : ICommand
    {
        //Передаем управление этим событием системе WPF:
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
        // Функция CanExecute возвращает либо истину либо ложь
        //Если приходит ложь, то команду выполнить нельзя и элемент откл автоматически
        //Если хочу выключить работу с каким-либо визуальным элементом, то у CanExecute возвращаем ложь
        public abstract bool CanExecute(object parameter);
        //Метод Execute - это то, что должно быть выполненно основной командой. Основная логика команды в Execute
        public abstract void Execute(object parameter);
    }
}
