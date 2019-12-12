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
        public void wasChangeHandler(object sender, DocumentChangedEventArgs args) {
            foreach (string trnName in args.GetTransactionNames()) {
                
                switch (trnName) {
                    case TransactionNames.EDIT_TEXT:
                        editTextHandler(args);
                        return;
                    
                    case TransactionNames.DRAG:
                        dragHandler(args);
                        return;
                }
            }
        }

        private void editTextHandler(DocumentChangedEventArgs args) {
            ICollection<ElementId> modifElemIds = args.GetModifiedElementIds();
            IEnumerable<Comment> textNoteToUpdate = Comments.Where(comm => modifElemIds.Contains(comm.TextNoteId));

            if (textNoteToUpdate.Count() > 0) {
                Document doc = args.GetDocument();
                
                Dictionary<ElementId, Element> modifElem = getElementsById(doc, textNoteToUpdate.Select(elem => elem.TextNoteId));
                foreach (var item in textNoteToUpdate) {
                    item.CommentText = ((TextNote)modifElem[item.TextNoteId]).Text;
                }
            }
        }

        private void dragHandler(DocumentChangedEventArgs args) {
            ICollection<ElementId> modifElemIds = args.GetModifiedElementIds();


            IEnumerable<ElementModel> elemsToUpdate = Comments.Select(comm => comm.Elements)
                                                              .SelectMany(x => x)
                                                              .Where(elem => modifElemIds.Contains(elem.Id));

            if (elemsToUpdate.Count() > 0) {
                Document doc = args.GetDocument();
                Dictionary<ElementId, Element> modifElem = getElementsById(doc, elemsToUpdate.Select(elem => elem.Id));

                Dictionary<Leader, XYZ> updateInfo = new Dictionary<Leader, XYZ>();

                foreach (var item in elemsToUpdate) {
                    item.Position = Helper.GetElementPosition(modifElem[item.Id]);
                    updateInfo[item.Leader] = item.Position;
                }

                Main.getInstance().Transactions.UpdateLeader(doc, updateInfo);
            }


            IEnumerable<Comment> updatedComments = Comments.Where(comm => modifElemIds.Contains(comm.TextNoteId));
            
            if (updatedComments.Count() > 0) {
                Document doc = args.GetDocument();
                var tmp = ((TextNote)doc.GetElement(updatedComments.First().TextNoteId)).GetLeaders();

                // It's necessary to recreate leaders because Revit creates new leaders each time when TextNote is moved
                // Error is occured when trying to update existed leaders
                Main.getInstance().Transactions.RemoveLeaders(doc, updatedComments.Select(comment => (TextNote)doc.GetElement(comment.TextNoteId)));
                
                Dictionary<TextNote, IEnumerable<ElementModel>> updateInfo = new Dictionary<TextNote, IEnumerable<ElementModel>>();

                foreach (var comment in updatedComments) {
                    updateInfo[(TextNote)doc.GetElement(comment.TextNoteId)] = comment.Elements;
                }

                Main.getInstance().Transactions.CreateLeaders(doc, updateInfo);
            }
        }


        public void addComment(Comment comment) {
            comments.Add(comment);
        }

        public void removeComment(Comment comment) {
            comments.Remove(comment);
        }

        public bool isCommentExists(Comment comment) {
            return Comments.Contains(comment);
        }

        // TODO: write doc
        // BUG: change way to store the result of func
        // because exeption is thrown when several leaders point the same object
        private Dictionary<ElementId, Element> getElementsById(Document doc, IEnumerable<ElementId> ids) {
            return ids.Select(elemId => doc.GetElement(elemId)).ToDictionary(elem => elem.Id);
        }


        public void RaiseCommentEditApply(object sender) {
            if (sender is Comment comment) {
                if (!isCommentExists(comment)) {
                    addComment(comment);
                    Main.getInstance().Transactions.CreateTextNote(comment);
                }
            }
            else {
                // TODO: write own exeption type
                throw new Exception("sender is not a Comment type");
            }
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
