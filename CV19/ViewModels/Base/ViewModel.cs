using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace CV19.ViewModels.Base
{
    internal abstract class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        //Метод Set, его задача обновлять значения св-ва, для которого определено поле в котором св-во хранит свои данные
        //ref T field - это ссылка на поле св-ва, T value - сюда передавать новое значение которое хотим установить,
        //[CallerMemberName] string PeopsertName = null - этот параметр будет самостоятельно определяться компилятором или вручную. Это имя сво-ва, которое мы передадим в OnPropertyChanged()
        
        protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string PropertyName = null)
        {
            //Убираем (возможное) закольцованное обновление сво-ва, которое нам не нужно и может пародить переполнение стека
            if(Equals(field, value)) return false; //Если поле которое мы хотиим изменить, уже содержит в себе новые данные, то ничего не делаем
            field = value;
            OnPropertyChanged(PropertyName);
            return true;
            //Если св-во изменилось, то с помощью флагов ложь или истина, мы можем выполнить ещё какую-нибудь работу (по обновлению других св-в,например, которые с ним связаны)

        }
    }
}
