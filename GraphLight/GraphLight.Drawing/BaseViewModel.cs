using System.ComponentModel;
using GraphLight.Annotations;

namespace GraphLight
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected void RaisePropertyChanged(string name)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }

        [NotifyPropertyChangedInvocator("name")]
        protected void SetProperty<T>(ref T field, T value, string name)
        {
            if(Equals(field, value))
                return;
            field = value;
            RaisePropertyChanged(name);
        }
    }
}