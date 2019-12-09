using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using TODOComm.Models;
using TODOComm.UI;

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
        public CommentEditCommand AddElementCommand { get; set; }
        public CommentEditCommand RemoveElementCommand { get; set; }

        public Comment Comment { get; set; }
        public bool isApply { get; private set; }

        private ElementModel selectedElement;
        public ElementModel SelectedElement {
            get {
                return selectedElement;
            }
            
            set {
                selectedElement = value;
                RemoveElementCommand.RaiseCanExecuteChanged();
            }
        }


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
                    isApply = false;
                    Comment.cancelChanges();
                    CloseWindowCommand.Execute();
                }
            };

            this.AddElementCommand = new CommentEditCommand() {
                act = () => {
                    CloseWindowCommand.Execute();
                    Comment.selectMultiElements();
                    CommentEdit win = new CommentEdit(Comment);
                    win.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                    win.ShowDialog();
                },
            };

            this.RemoveElementCommand = new CommentEditCommand() {
                act = () => {
                    Comment.removeElement(SelectedElement);
                },
                func = () => SelectedElement != null
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
