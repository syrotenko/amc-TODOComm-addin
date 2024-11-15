﻿using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using TODOComm.Helper;

namespace TODOComm.Models {
    public class Comment : INotifyPropertyChanged {
        public Comment(UIDocument uiDoc) {
            this.Elements = new ObservableCollection<ElementModel>();
            this.doc = uiDoc.Document;
            this.view = uiDoc.ActiveView;
            this.selection = uiDoc.Selection;
            Prior = default;
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

                // It's necessary to create leaders here because TextNote is required
                if (IsVisibleLeaders) {
                    Dictionary<TextNote, IEnumerable<ElementModel>> updateInfo = new Dictionary<TextNote, IEnumerable<ElementModel>>() {
                        { (TextNote)doc.GetElement(TextNoteId), Elements }
                    };

                    Main.getInstance().Transactions.CreateLeaders(doc, updateInfo);
                }

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

        private bool isVisibleLeaders = true;
        public bool IsVisibleLeaders {
            get {
                return isVisibleLeaders;
            }
            set {
                isVisibleLeaders = value;

                if (value)
                    showLeaders();
                else
                    hideLeaders();

                OnPropertyChanged(PropertyNames.IS_VISIBLE_LEADERS);
            }
        }

        private Priority prior;
        public Priority Prior {
            get {
                return prior;
            }

            set {
                prior = value;
                OnPropertyChanged(PropertyNames.PRIOR);
            }
        }


        public Document doc;
        public View view;
        public Selection selection;


        public void addElements(IEnumerable<ElementModel> elements) {
            // Check if all elements have not were not already been added
            if (!elements.All(element => !isElementAdded(element.Id))) {
                throw new ArgumentException("Element with this Id has already been added");
            }

            foreach (ElementModel element in elements) {
                Elements.Add(element);
            }
            
            if (TextNoteId != null && IsVisibleLeaders) {
                Dictionary<TextNote, IEnumerable<ElementModel>> updateInfo = new Dictionary<TextNote, IEnumerable<ElementModel>>() {
                    { (TextNote)doc.GetElement(TextNoteId), elements }
                };

                Main.getInstance().Transactions.CreateLeaders(doc, updateInfo);
            }
        }

        public void removeElement(ElementModel element) {
            Elements.Remove(element);

            if (IsVisibleLeaders) {
                hideLeaders();
                showLeaders();
            }
        }

        public void applyChanges() {
            TODOCommModel.getInstance().RaiseCommentEditApply(this);
            prevCommentText = CommentText;
            if (TextNoteId != null && doc != null) {
                Main.getInstance().Transactions.ChangeTextNoteText(doc, TextNoteId, CommentText);
            }
        }

        public void cancelChanges() {
            CommentText = prevCommentText;
        }

        public bool isTextNoteExist(ElementId textNoteIdOther) {
            return TextNoteId.Equals(textNoteIdOther);
        }

        public bool isElementAdded(ElementId elementIdOther) {
            return Elements.Any(element => element.Id.Equals(elementIdOther));
        }

        public void highlightComment() {
            List<ElementId> elemIdsToHighlight = Elements.Select(x => x.Id).ToList();
            elemIdsToHighlight.Add(TextNoteId);

            selection.SetElementIds(elemIdsToHighlight);
        }

        public void pickElement() {
            Reference objRef = selection.PickObject(ObjectType.Element, Prompts.SELECT_OBJ);
            Element element = doc.GetElement(objRef);

            // Handling case when picked objects are not physical elements
            try {
                ElementModel elementModel = new ElementModel(element.Id, element.Name, HelperClass.GetElementPosition(element));
                addElements(new List<ElementModel>() { elementModel });
            }
            catch (ArgumentException ex) {
                throw new ArgumentException(ex.Message);
            }
        }

        public void pickMultiElements () {
            IList<Reference> objRefs = selection.PickObjects(ObjectType.Element, Prompts.SELECT_OBJS);
            
            IEnumerable<Element> elements = objRefs.Select(objRef => doc.GetElement(objRef));
            
            // Handling case when picked objects are not physical elements
            try {
                IEnumerable<ElementModel> elementModels = elements.Select(element => new ElementModel(element.Id, element.Name, HelperClass.GetElementPosition(element)));
                addElements(elementModels);
            }
            catch (ArgumentException ex) {
                throw new ArgumentException(ex.Message);
            }
        }

        private void showElements() {
            Main.getInstance().Transactions.ShowElements(doc, view, new List<ElementId>() { TextNoteId });
        }

        private void hideElements() {
            Main.getInstance().Transactions.HideElements(doc, view, new List<ElementId>() { TextNoteId });
        }

        private void showLeaders() {
            Dictionary<TextNote, IEnumerable<ElementModel>> updateInfo = new Dictionary<TextNote, IEnumerable<ElementModel>>() {
                        { (TextNote)doc.GetElement(TextNoteId), Elements }
                    };

            Main.getInstance().Transactions.CreateLeaders(doc, updateInfo);
        }

        private void hideLeaders() {
            Main.getInstance().Transactions.RemoveLeaders(doc, new List<TextNote>() { (TextNote)doc.GetElement(TextNoteId) });
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
            public const string IS_VISIBLE_LEADERS = "IsVisibleLeaders";
            public const string PRIOR = "Prior";
        }
    }
}
