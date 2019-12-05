using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace TODOComm.Models {
    class TODOCommModel : INotifyPropertyChanged {
        private TODOCommModel() {
            this.comments = new ObservableCollection<Comment>();
            Main.thisApp.application.ControlledApplication.DocumentChanged += new EventHandler<DocumentChangedEventArgs>(wasChange);
        }

        private static TODOCommModel instance;

        public static TODOCommModel getInstance() {
            if (instance == null)
                instance = new TODOCommModel();
            return instance;
        }

        private ObservableCollection<Comment> comments;
        public ObservableCollection<Comment> Comments {
            get {
                return this.comments;
            }
            set {
                this.comments = value;
                this.OnPropertyChanged(PropertyNames.COMMENTS);
            }
        }

        
        public void wasChange(object sender, DocumentChangedEventArgs args) {
            if (args.GetTransactionNames().Contains(TransactionNames.EDIT_TEXT)) {

                ICollection<ElementId> modifElemIds = args.GetModifiedElementIds();
                IEnumerable<ElementId> watchableElemIds = modifElemIds.Where(modifElemId => this.isWatchForTextNote(modifElemId));

                if (watchableElemIds.Count() > 0) {
                    Document doc = args.GetDocument();
                    this.updateCommentText(doc, watchableElemIds);
                }
            }
        }

        public void addComment(Comment comment) {
            this.comments.Add(comment);
            this.OnPropertyChanged(PropertyNames.COMMENTS);
        }

        public void removeComment(Comment comment) {
            this.comments.Remove(comment);
            this.OnPropertyChanged(PropertyNames.COMMENTS);
        }

        
        private void updateCommentText(Document doc, IEnumerable<ElementId> textNoteIds) {
            Dictionary<ElementId, Element> modifiedElem = this.getWatchableElementById(doc, textNoteIds);
            Dictionary<ElementId, Comment> commentsToUpdate = this.getCommentsByTextNoteId(textNoteIds);

            foreach (KeyValuePair<ElementId, Element> entry in modifiedElem) {
                commentsToUpdate[entry.Key].CommentText = ((TextNote)entry.Value).Text;
            }
        }

        private bool isWatchForTextNote(ElementId textNoteId) {
            return this.comments.Any(comm => comm.isTextNoteExist(textNoteId));
        }

        private Dictionary<ElementId, Element> getWatchableElementById(Document doc, IEnumerable<ElementId> ids) {
            return ids.Select(elemId => doc.GetElement(elemId)).ToDictionary(elem => elem.Id);
        }

        private Dictionary<ElementId, Comment> getCommentsByTextNoteId(IEnumerable<ElementId> textNoteIds) {
            return textNoteIds.Select(elem => this.getCommentByTextNoteId(elem)).ToDictionary(elem => elem.TextNoteId);
        }

        private Comment getCommentByTextNoteId(ElementId textNoteId) {
            return this.comments.Where(comm => comm.isTextNoteExist(textNoteId)).First();
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static class PropertyNames {
            public static string COMMENTS = "Comments";
        }
    }
}
