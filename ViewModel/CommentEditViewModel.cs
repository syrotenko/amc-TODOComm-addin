using System;
using System.ComponentModel;
using System.Windows.Input;
using TODOComm.Models;

namespace TODOComm.ViewModel {
    public class CommentEditViewModel : INotifyPropertyChanged {
        public CommentEditViewModel(Comment comment) {
            this.Comment = comment;
            
            this.setupCommands();

            Comment.PropertyChanged += RaisePropertyChanged;
        }

        public CommentEditCommand ApplyChangesCommand { get; set; }
        public CommentEditCommand CancelChangesCommand { get; set; }
        public CommentEditCommand CloseWindowCommand { get; set; }
        
        public Comment Comment { get; set; }

        public bool isApply { get; private set; }


        private void RaisePropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (string.Compare(e.PropertyName, Comment.PropertyNames.COMMENT_TEXT) == 0) {
                ApplyChangesCommand.RaiseCanExecuteChanged();
            }
        }


        private void setupCommands() {
            this.ApplyChangesCommand = new CommentEditCommand() {
                act = () => {
                    this.isApply = true;
                    this.Comment.applyChanges();
                    this.CloseWindowCommand.Execute();
                },
                func = () => !string.IsNullOrEmpty(this.Comment.CommentText)
            };

            this.CancelChangesCommand = new CommentEditCommand() {
                act = () => {
                    this.isApply = false;
                    this.Comment.cancelChanges();
                    this.CloseWindowCommand.Execute();
                }
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class CommentEditCommand : ICommand {
        public Action act;
        public Func<bool> func;

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged() {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        public bool CanExecute(object parameter) {
            if (func != null)
                return func();
            return true;
        }

        public void Execute(object parameter) {
            act?.Invoke();
        }

        public void Execute() {
            act?.Invoke();
        }
    }
}
