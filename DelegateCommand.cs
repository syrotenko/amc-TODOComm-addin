using System;
using System.Windows.Input;

namespace TODOComm {
    public class DelegateCommand : ICommand {
        public Action<object> act;
        public Func<bool> canExec;

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged() {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        public bool CanExecute(object parameter) {
            if (canExec != null)
                return canExec();
            return true;
        }

        public void Execute(object parameter) {
            act?.Invoke(parameter);
        }

        public void Execute() {
            act?.Invoke(new object());
        }
    }
}
