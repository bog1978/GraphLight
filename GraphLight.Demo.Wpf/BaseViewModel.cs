using System.ComponentModel;
using System.Runtime.CompilerServices;
using GraphLight.Annotations;

namespace GraphLight.Demo
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator("name")]
        protected void RaisePropertyChanged([CallerMemberName] string name = null) => 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        [NotifyPropertyChangedInvocator("name")]
        protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string name = null)
        {
            if(Equals(field, value))
                return;
            field = value;
            RaisePropertyChanged(name);
        }
    }
}