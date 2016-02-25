using System;
using System.Windows.Input;

namespace GraphLight
{
    public class DelegateCommand<T> : ICommand
    {
        private readonly Action<T> _action;
        private readonly Func<T,bool> _canExecute;
        public DelegateCommand(Action<T> action)
        {
            _action = action;
            _canExecute = x => true;
        }

        public DelegateCommand(Action<T> action, Func<T,bool> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute((T)parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action((T)parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }
    }

    public class DelegateCommand : ICommand
    {
        private readonly Action _action;
        private readonly Func<bool> _canExecute;
        public DelegateCommand(Action action)
        {
            _action = action;
            _canExecute = () => true;
        }

        public DelegateCommand(Action action, Func<bool> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute();
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action();
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }
    }
}
