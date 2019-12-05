﻿using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace TODOComm.Models {
    public class Comment : INotifyPropertyChanged {
        public Comment(Document doc) {
            this.Elements = new ObservableCollection<ElementModel>();
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
                this.commentPosition = value;
                OnPropertyChanged(PropertyNames.COMMENT_POSITION);
            }
        }

        public Document doc;


        public void addElement(ElementModel element) {
            this.Elements.Add(element);
        }

        public void removeElement(ElementModel element) {
            this.Elements.Remove(element);
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
            public static string TEXTNOTE_ID = "TextNoteId";
            public static string ELEMENTS = "Elements";
            public static string COMMENT_POSITION = "CommentPosition";
        }
    }
}
