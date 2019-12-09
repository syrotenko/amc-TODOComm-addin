using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
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

        private bool isVisible = true;
        public bool IsVisible {
            get {
                return isVisible;
            }
            set {
                isVisible = value;

                if (value)
                    showElements();
                else
                    hideElements();

                OnPropertyChanged(PropertyNames.IS_VISIBLE);
            }
        }


        public Document doc;
        public UIDocument uiDoc;


        public void addElement(ElementModel element) {
            Elements.Add(element);
        }

        public void removeElement(ElementModel element) {
            Elements.Remove(element);
        }

        public void applyChanges() {
            TODOCommModel.getInstance().RaiseCommentEditApply(this);
            prevCommentText = CommentText;
            if (TextNoteId != null && doc != null) {
                Main.ExternalApp.Transactions.ChangeTextNoteText(doc, TextNoteId, CommentText);
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

        public void pickElement() {
            Selection selection = uiDoc.Selection;
            Reference objRef = selection.PickObject(ObjectType.Element, Prompts.SELECT_OBJ);
            Element elem = doc.GetElement(objRef);

            addElement(new ElementModel(elem.Id, elem.Name));
        }

        public void pickMultiElements () {
            Selection selection = uiDoc.Selection;
            IList<Reference> objRefs = selection.PickObjects(ObjectType.Element, Prompts.SELECT_OBJS);
            foreach (Reference objRef in objRefs) {
                Element elem = doc.GetElement(objRef);
                addElement(new ElementModel(elem.Id, elem.Name));
            }
        }

        private void showElements() {
            Main.ExternalApp.Transactions.ShowElements(doc, uiDoc.ActiveView, new List<ElementId>() { TextNoteId });
        }

        private void hideElements() {
            Main.ExternalApp.Transactions.HideElements(doc, uiDoc.ActiveView, new List<ElementId>() { TextNoteId });
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
            public const string IS_VISIBLE = "IsVisible";
        }
    }
}
