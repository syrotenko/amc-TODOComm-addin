using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using TODOComm.Models;
using TODOComm.UI;
using TODOComm.Helper;

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


        public DelegateCommand EditComm { get; set; }
        public DelegateCommand SelectComm { get; set; }
        public DelegateCommand SetVisibleComm { get; set; }
        public DelegateCommand SetVisibleLeaders { get; set; }


        private TODOCommModel TODOCommModel;


        private void setupCommands() {
            EditComm = new DelegateCommand() {
                act = (obj) => {
                    if (obj is Comment comment) {
                        CommentEdit win = new CommentEdit(comment);
                        win.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                        win.ShowDialog();
                    }
                    else {
                        throw new NotSupportedException("Argument is not Comment type");
                    }
                }
            };

            SelectComm = new DelegateCommand() {
                act = (obj) => {
                    if (obj is Comment comment) {
                        SelectedComment = comment;
                        comment.highlightComment();
                    }
                    else {
                        throw new NotSupportedException("Argument is not Comment type");
                    }
                }
            };

            SetVisibleComm = new DelegateCommand() {
                act = (obj) => {
                    if (obj is Comment comment) {
                        comment.IsVisible = comment.IsVisible == true ? false : true;
                    }
                    else {
                        throw new NotSupportedException("Argument is not Comment type");
                    }
                }
            };

            SetVisibleLeaders = new DelegateCommand() {
                act = (obj) => {
                    if (obj is Comment comment) {
                        comment.IsVisibleLeaders = comment.IsVisibleLeaders == true ? false : true;
                    }
                    else {
                        throw new NotSupportedException("Argument is not Comment type");
                    }
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
}
