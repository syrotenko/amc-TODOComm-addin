using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using TODOComm.Models;

namespace TODOComm.UI {
    class TODOCommPaneViewModel : INotifyPropertyChanged {
        public TODOCommPaneViewModel() {
            this.TODOCommModel = TODOCommModel.getInstance();
            this.setupCommands();
            this.comments = TODOCommModel.Comments;
        }

        private ObservableCollection<Comment> comments;
        public ObservableCollection<Comment> Comments {
            get { return comments; }
            set { comments = value; }
        }


        private Comment selectedComment;
        public Comment SelectedComment {
            get { return selectedComment; }
            set { selectedComment = value; }
        }


        public BtnCommands EditComm { get; set; }
        public BtnCommands SelectComm { get; set; }

        public TODOCommModel TODOCommModel { get; set; }

        private void setupCommands() {
            this.EditComm = new BtnCommands() {
                act = (comm) => {
                    WindowMain win = new WindowMain(comm);
                    win.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                    win.ShowDialog();
                }
            };

            this.SelectComm = new BtnCommands() {
                act = (comm) => {
                    comm.highlightComment();
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

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) {
            return true;
        }

        public void Execute(object parameter) {
            act?.Invoke((Comment)parameter);
        }
    }
}
