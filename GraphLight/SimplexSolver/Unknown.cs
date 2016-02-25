using GraphLight.Graph;

namespace WpfSimplexSolver
{
    public class Unknown : BaseViewModel
    {
        private string _name;
        private double _value;
        private double _weight;

        public Unknown(int cnt, double weight)
        {
            Name = "X" + cnt;
            Weight = weight;
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        public double Weight
        {
            get { return _weight; }
            set
            {
                _weight = value;
                RaisePropertyChanged("Weight");
            }
        }

        public double Value
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