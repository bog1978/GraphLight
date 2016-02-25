using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GraphLight.Algorithm;
using GraphLight.Graph;

namespace WpfSimplexSolver
{
    public class SimplexSolverViewModel : BaseViewModel
    {
        private int _cnt = 1;

        public SimplexSolverViewModel()
        {
            UnknownCollection = new ObservableCollection<Unknown>();
            RestrictionCollection = new ObservableCollection<RestrictionRow>();
            SolutionCollection = new ObservableCollection<Cell<double>>();
            SelectedTask = TaskCollection[0];
            AddUnknown();
            AddUnknown();
            AddUnknown();
            AddRestriction();
        }

        public ObservableCollection<Unknown> UnknownCollection { get; private set; }
        public ObservableCollection<Cell<double>> SolutionCollection { get; private set; }
        public ObservableCollection<RestrictionRow> RestrictionCollection { get; private set; }
        public List<OptimizationTask> TaskCollection
        {
            get { return OptimizationTask.Values; }
        }

        private OptimizationTask _selectedTask;
        public OptimizationTask SelectedTask
        {
            get { return _selectedTask; }
            set
            {
                _selectedTask = value;
                RaisePropertyChanged("SelectedTask");
            }
        }

        private double _result;
        public double Result
        {
            get { return _result; }
            set
            {
                _result = value;
                RaisePropertyChanged("Result");
            }
        }
        public void AddUnknown()
        {
            UnknownCollection.Add(new Unknown(_cnt++, 1));
            SolutionCollection.Add(new Cell<double>(){Value = -1});
            foreach (var restrictionRow in RestrictionCollection)
            {
                restrictionRow.WeightCollection.Add(new Cell<double>());
            }
        }

        public void AddRestriction()
        {
            RestrictionCollection.Add(new RestrictionRow(UnknownCollection.Count));
        }

        public void Solve()
        {
            var simplex = new SimplexSolver();
            foreach (var restrictionRow in RestrictionCollection)
            {
                simplex.AddConstraint(
                    restrictionRow.SelectedRestrictionSign.Operation,
                    restrictionRow.ResctrictionValue,
                    restrictionRow.WeightCollection.Select(x => x.Value).ToArray());
            }
            Result = simplex.Solve(SelectedTask.Task, UnknownCollection.Select(x => x.Weight).ToArray());
            for (var i = 0; i < UnknownCollection.Count; i++)
            {
                UnknownCollection[i].Value = simplex.Solution[i];
            }
        }
    }
}