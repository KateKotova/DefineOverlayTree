using System;
using System.Windows.Input;

namespace DefineOverlayTree.HelperClasses
{
    public class DelegateCommand<T> : ICommand
    {
        private readonly Action<T> m_Execute;
        private readonly Func<T, bool> m_CanExecute;

        public DelegateCommand(Action<T> execute)
            : this(execute, f => true)
        {
        }

        public DelegateCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            m_Execute = execute;
            m_CanExecute = canExecute;
        }

        public void Execute()
        {
            Execute(null);
        }

        public void Execute(T parameter)
        {
            m_Execute?.Invoke(parameter);
        }

        public void Execute(object parameter)
        {
            if (parameter == null || parameter is T)
            {
                Execute((T)parameter);
            }

        }

        public bool CanExecute()
        {
            return CanExecute(null);
        }

        public bool CanExecute(T parameter)
        {
            return m_CanExecute == null || m_CanExecute(parameter);
        }

        public bool CanExecute(object parameter)
        {
            if (parameter == null || parameter is T)
            {
                return CanExecute((T)parameter);
            }
            return false;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (m_CanExecute != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {
                if (m_CanExecute != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
