using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using TODOComm.Models;

namespace TODOComm.UI {
    class TODOCommPaneViewModel : INotifyPropertyChanged {
        public TODOCommPaneViewModel() {
            this.TODOCommModel = TODOCommModel.getInstance();
            this.setupCommands();
        }

        public BtnCommands EditComm { get; set; }

        public TODOCommModel TODOCommModel { get; set; }

        private void setupCommands() {
            this.EditComm = new BtnCommands() {
                act = (comm) => {
                    WindowMain win = new WindowMain(comm);
                    win.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                    win.ShowDialog();
                }
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class BtnCommands : ICommand {
        public Action<Comment> act;
        //public Func<bool> func;

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) {
            return true;
        }

        public void Execute(object parameter) {
            act?.Invoke((Comment)parameter);
        }
    }
}
