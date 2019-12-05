using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace TODOComm.Models {
    public class Comment : INotifyPropertyChanged {
        public Comment(Document doc) {
            this.Elements = new ObservableCollection<Element>();
            this.doc = doc;
        }

        private string commentText;
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

        //private TextNote textNote;
        //public TextNote TextNote {
        //    get {
        //        return this.textNote;
        //    }
        //    set {
        //        this.textNote = value;
        //        OnPropertyChanged(PropertyNames.TEXTNOTE);
        //    }
        //}

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

        private ObservableCollection<Element> elements;
        public ObservableCollection<Element> Elements {
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
                this.commentPosition = value;
                OnPropertyChanged(PropertyNames.COMMENT_POSITION);
            }
        }

        public Document doc;


        public void addElement(Element element) {
            this.Elements.Add(element);
            this.OnPropertyChanged(PropertyNames.ELEMENTS);
        }

        public void removeElement(Element element) {
            this.Elements.Remove(element);
            this.OnPropertyChanged(PropertyNames.ELEMENTS);
        }

        public void applyChanges() {
            this.prevCommentText = CommentText;
            if (this.TextNoteId != null && this.doc != null) {
                Main.thisApp.changeTextNoteText(doc, TextNoteId, CommentText);
            }
        }

        public void cancelChanges() {
            this.CommentText = this.prevCommentText;
        }

        public bool isTextNoteExist(ElementId textNoteIdOther) {
            return this.TextNoteId.Equals(textNoteIdOther);
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static class PropertyNames {
            public static string COMMENT_TEXT = "CommentText";
            //public static string TEXTNOTE = "TextNote";
            public static string TEXTNOTE_ID = "TextNoteId";
            public static string ELEMENTS = "Elements";
            public static string COMMENT_POSITION = "CommentPosition";
        }
    }

    class ExtCommand : IExternalCommand {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements) {
            throw new System.NotImplementedException();
        }
    }
}
