using System.Collections.Generic;
using System.Collections.ObjectModel;
using GraphLight.Graph;

namespace WpfSimplexSolver
{
    public class RestrictionRow : BaseViewModel
    {
        private double _resctrictionValue;
        private RestrictionSign _selectedRestrictionSign;

        public RestrictionRow(int unknownCount)
        {
            WeightCollection = new ObservableCollection<Cell<double>>();
            for (var i = 0; i < unknownCount; i++)
                WeightCollection.Add(new Cell<double>());
            SelectedRestrictionSign = RestrictionSignCollection[0];
        }

        public List<RestrictionSign> RestrictionSignCollection
        {
            get { return RestrictionSign.Values; }
        }

        public ObservableCollection<Cell<double>> WeightCollection { get; private set; }

        public double ResctrictionValue
        {
            get { return _resctrictionValue; }
            set
            {
                _resctrictionValue = value;
                RaisePropertyChanged("ResctrictionValue");
            }
        }

        public RestrictionSign SelectedRestrictionSign
        {
            get { return _selectedRestrictionSign; }
            set
            {
                _selectedRestrictionSign = value;
                RaisePropertyChanged("SelectedRestrictionSign");
            }
        }
    }
}