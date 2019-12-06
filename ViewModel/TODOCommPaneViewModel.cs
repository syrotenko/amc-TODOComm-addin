using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using TODOComm.Models;
using TODOComm.UI;

namespace TODOComm.ViewModel {
    class TODOCommPaneViewModel : INotifyPropertyChanged {
        public TODOCommPaneViewModel() {
            TODOCommModel = TODOCommModel.getInstance();
            setupCommands();
            comments = TODOCommModel.Comments;
        }

        private ObservableCollection<Comment> comments;
        public ObservableCollection<Comment> Comments {
            get { return comments; }
            set { comments = value; }
        }

        private Comment selectedComment;
        public Comment SelectedComment {
            get { 
                return selectedComment; 
            }
            set { 
                selectedComment = value;
                OnPropertyChanged(PropertyNames.SELECTED_COMMNET);
            }
        }


        public BtnCommands EditComm { get; set; }
        public BtnCommands SelectComm { get; set; }
        public BtnCommands SetVisibleComm { get; set; }


        private TODOCommModel TODOCommModel;


        private void setupCommands() {
            EditComm = new BtnCommands() {
                act = (comment) => {
                    CommentEdit win = new CommentEdit(comment);
                    win.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                    win.ShowDialog();
                }
            };

            SelectComm = new BtnCommands() {
                act = (comment) => {
                    SelectedComment = comment;
                    comment.highlightComment();
                }
            };

            SetVisibleComm = new BtnCommands() {
                act = (comment) => {
                    comment.IsVisible = comment.IsVisible == true ? false : true;
                }
            };
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public static class PropertyNames {
            public const string SELECTED_COMMNET = "SelectedComment";
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
