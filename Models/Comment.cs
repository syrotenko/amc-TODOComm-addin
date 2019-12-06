using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace TODOComm.Models {
    public class Comment : INotifyPropertyChanged {
        public Comment(UIDocument uiDoc) {
            this.Elements = new ObservableCollection<ElementModel>();
            this.uiDoc = uiDoc;
            this.doc = uiDoc.Document;
        }

        
        private string commentText;
        // TODO: save all state of this object, not only comment text
        private string prevCommentText;
        public string CommentText {
            get {
                return commentText;
            }
            set {
                this.commentText = value;
                OnPropertyChanged(PropertyNames.COMMENT_TEXT);
            }
        }

        private ElementId textNoteId;
        public ElementId TextNoteId {
            get {
                return this.textNoteId;
            }
            set {
                this.textNoteId = value;
                OnPropertyChanged(PropertyNames.TEXTNOTE_ID);
            }
        }

        private ObservableCollection<ElementModel> elements;
        public ObservableCollection<ElementModel> Elements {
            get {
                return elements;
            }
            set {
                this.elements = value;
                OnPropertyChanged(PropertyNames.ELEMENTS);
            }
        }

        private XYZ commentPosition;
        public XYZ CommentPosition {
            get {
                return commentPosition;
            }
            set {
                commentPosition = value;
                OnPropertyChanged(PropertyNames.COMMENT_POSITION);
            }
        }


        private Document doc;
        private UIDocument uiDoc;


        public void addElement(ElementModel element) {
            Elements.Add(element);
        }

        public void removeElement(ElementModel element) {
            Elements.Remove(element);
        }

        public void applyChanges() {
            prevCommentText = CommentText;
            if (TextNoteId != null && doc != null) {
                Main.ExternalApp.changeTextNoteText(doc, TextNoteId, CommentText);
            }
        }

        public void cancelChanges() {
            CommentText = prevCommentText;
        }

        public bool isTextNoteExist(ElementId textNoteIdOther) {
            return TextNoteId.Equals(textNoteIdOther);
        }
        public void highlightComment() {
            List<ElementId> elemIdsToHighlight = Elements.Select(x => x.Id).ToList();
            elemIdsToHighlight.Add(TextNoteId);

            uiDoc.Selection.SetElementIds(elemIdsToHighlight);
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static class PropertyNames {
            public const string COMMENT_TEXT = "CommentText";
            public const string TEXTNOTE_ID = "TextNoteId";
            public const string ELEMENTS = "Elements";
            public const string COMMENT_POSITION = "CommentPosition";
        }
    }
}
