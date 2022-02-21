using CV19.Infrastructure.Commands.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace CV19.Infrastructure.Commands
{
    internal class LambdaCommand : Command
    {
        //Здесь мы сохраняем то, что пришло из конструктора, в приватные поля
        //readonly - благодаря этому параметру, данные не изменят и отрабатывать с ним программа будет быстрее 
        private readonly Action<object> _Execute;
        private readonly Func<object, bool> _CanExecute;

        //Func<object, bool> CanExecute = null --- сделано для того, чтобы мы могли не указывать этот второй параметр
        public LambdaCommand(Action<object> Execute,Func<object, bool> CanExecute = null)
        {
            //Делаем проверку и ругаемся, если нам не дали ссылку на делегат Action<object> Execute
            _Execute = Execute ?? throw new ArgumentNullException(nameof(Execute)) ;
            _CanExecute = CanExecute;
        }
        //вызывая CanExecute подразумевая, что там может быть пустая ссылка. И если нет этого делигата, то считаем, что команду можно выполнить в любом случае
        public override bool CanExecute(object parameter) => _CanExecute?.Invoke(parameter) ?? true;

        public override void Execute(object parameter) => _Execute(parameter);
    }
}
