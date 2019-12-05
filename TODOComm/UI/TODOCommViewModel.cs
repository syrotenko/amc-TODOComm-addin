using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using TODOComm.Models;

namespace TODOComm.UI {
    public class TODOCommViewModel : INotifyPropertyChanged {
        public TODOCommViewModel(Comment comment) {
            this.Comment = comment;
            
            this.setupCommands();
            this.strs = new ObservableCollection<string>() { "path", "s", "qwe" };

            Comment.PropertyChanged += (s, e) => { RaisePropertyChanged(s, e); };
        }

        public ControlWindowCommands ApplyChangesCommand { get; set; }
        public ControlWindowCommands CancelChangesCommand { get; set; }
        public ControlWindowCommands CloseWindowCommand { get; set; }
        public Comment Comment { get; set; }

        public bool isApply { get; private set; }


        public ObservableCollection<string> strs { get; set; }

        private void RaisePropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (string.Compare(e.PropertyName, Comment.PropertyNames.COMMENT_TEXT) == 0) {
                ApplyChangesCommand.RaiseCanExecuteChanged();
            }
        }


        private void setupCommands() {
            this.ApplyChangesCommand = new ControlWindowCommands() {
                act = () => {
                    this.isApply = true;
                    this.Comment.applyChanges();
                    this.CloseWindowCommand.Execute();
                },
                func = () => !string.IsNullOrEmpty(this.Comment.CommentText)
            };

            this.CancelChangesCommand = new ControlWindowCommands() {
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

    public class ControlWindowCommands : ICommand {
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
