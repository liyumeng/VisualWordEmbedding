using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NewsSpectrum.Utils
{
    public class ExecutingEventArgs : EventArgs
    {
        public object Parameter { get; private set; }

        public ExecutingEventArgs(object parameter)
        {
            Parameter = parameter;
        }
    }
    public class ActionCommand : ICommand
    {
        private bool m_isEnabled;

        public bool IsEnabled
        {
            get { return m_isEnabled; }
            set
            {
                m_isEnabled = value;
                OnCanExecuteChanged();
            }
        }

        public event EventHandler<ExecutingEventArgs> Executing;

        private void OnExecuting(ExecutingEventArgs e)
        {
            if (Executing != null)
            {
                Executing(this, e);
            }
        }

        private void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        public bool CanExecute(object parameter)
        {
            return IsEnabled;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            OnExecuting(new ExecutingEventArgs(parameter));
        }

        public ActionCommand()
        {
            IsEnabled = true;
        }
    }
}
