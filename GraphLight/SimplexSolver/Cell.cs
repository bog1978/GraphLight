using GraphLight.Graph;

namespace WpfSimplexSolver
{
    public class Cell<T> : BaseViewModel
    {
        private T _value;

        public T Value
        {
            get { return _value; }
            set
            {
                _value = value;
                RaisePropertyChanged("Value");
            }
        }
    }
}