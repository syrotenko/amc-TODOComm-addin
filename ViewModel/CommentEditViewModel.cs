using System.ComponentModel;
using TODOComm.Models;
using TODOComm.UI;
using TODOComm.Helper;
using System;
using Autodesk.Revit.UI;

namespace TODOComm.ViewModel {

    public class CommentEditViewModel : INotifyPropertyChanged {

        public CommentEditViewModel(Comment comment) {
            this.Comment = comment;

            this.setupCommands();

            Comment.PropertyChanged += RaisePropertyChanged;
        }

        
        public DelegateCommand ApplyChangesCommand { get; set; }
        public DelegateCommand CancelChangesCommand { get; set; }
        public DelegateCommand CloseWindowCommand { get; set; }
        public DelegateCommand AddElementCommand { get; set; }
        public DelegateCommand RemoveElementCommand { get; set; }

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
            switch (e.PropertyName) {
                case Comment.PropertyNames.COMMENT_TEXT:
                    ApplyChangesCommand.RaiseCanExecuteChanged();
                    break;
            }
        }


        private void setupCommands() {
            this.ApplyChangesCommand = new DelegateCommand() {
                act = (dummy) => {
                    this.isApply = true;
                    this.Comment.applyChanges();
                    this.CloseWindowCommand.Execute();
                },
                canExec = () => !string.IsNullOrEmpty(this.Comment.CommentText)
            };

            this.CancelChangesCommand = new DelegateCommand() {
                act = (dummy) => {
                    isApply = false;
                    Comment.cancelChanges();
                    CloseWindowCommand.Execute();
                }
            };

            this.AddElementCommand = new DelegateCommand() {
                act = (dummy) => {
                    CloseWindowCommand.Execute();
                    try {
                        Comment.pickMultiElements();
                    }
                    catch (Autodesk.Revit.Exceptions.OperationCanceledException) { }
                    catch (ArgumentException ex) {
                        TaskDialog.Show("Error", ex.Message);
                    }

                    CommentEdit win = new CommentEdit(Comment);
                    win.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                    win.ShowDialog();
                },
            };

            this.RemoveElementCommand = new DelegateCommand() {
                act = (dummy) => {
                    Comment.removeElement(SelectedElement);
                },
                canExec = () => SelectedElement != null
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
