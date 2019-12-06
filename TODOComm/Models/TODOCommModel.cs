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
            comments = new ObservableCollection<Comment>();
            Main.ExternalApp.registerDocumentChanged(new EventHandler<DocumentChangedEventArgs>(wasChange));
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
                return comments;
            }
            private set {
                comments = value;
                OnPropertyChanged(PropertyNames.COMMENTS);
            }
        }

        // TODO: make documentation
        public void wasChange(object sender, DocumentChangedEventArgs args) {
            if (args.GetTransactionNames().Contains(TransactionNames.EDIT_TEXT)) {

                ICollection<ElementId> modifElemIds = args.GetModifiedElementIds();
                IEnumerable<ElementId> watchableElemIds = modifElemIds.Where(modifElemId => isWatchForTextNote(modifElemId));

                if (watchableElemIds.Count() > 0) {
                    Document doc = args.GetDocument();
                    updateCommentText(doc, watchableElemIds);
                }
            }
        }

        public void addComment(Comment comment) {
            comments.Add(comment);
        }

        public void removeComment(Comment comment) {
            comments.Remove(comment);
        }

        // TODO: write that it's not necessary to check if key exists because it's a private method
        private void updateCommentText(Document doc, IEnumerable<ElementId> textNoteIds) {
            Dictionary<ElementId, Element> modifiedElem = getWatchableElementById(doc, textNoteIds);
            Dictionary<ElementId, Comment> commentsToUpdate = getCommentsByTextNoteId(textNoteIds);

            foreach (KeyValuePair<ElementId, Element> entry in modifiedElem) {
                commentsToUpdate[entry.Key].CommentText = ((TextNote)entry.Value).Text;
            }
        }

        // TODO: write doc
        private bool isWatchForTextNote(ElementId textNoteId) {
            return this.comments.Any(comm => comm.isTextNoteExist(textNoteId));
        }

        // TODO: write doc
        private Dictionary<ElementId, Element> getWatchableElementById(Document doc, IEnumerable<ElementId> ids) {
            return ids.Select(elemId => doc.GetElement(elemId)).ToDictionary(elem => elem.Id);
        }

        // TODO: write doc
        private Dictionary<ElementId, Comment> getCommentsByTextNoteId(IEnumerable<ElementId> textNoteIds) {
            return textNoteIds.Select(elem => this.getCommentByTextNoteId(elem)).ToDictionary(elem => elem.TextNoteId);
        }

        // TODO: write doc
        private Comment getCommentByTextNoteId(ElementId textNoteId) {
            return this.comments.Where(comm => comm.isTextNoteExist(textNoteId)).First();
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static class PropertyNames {
            public const string COMMENTS = "Comments";
        }
    }
}
