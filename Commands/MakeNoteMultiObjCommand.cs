﻿using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using TODOComm.Models;
using TODOComm.UI;

namespace TODOComm.Commands {
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    class MakeNoteMultiObjCommand : CommandParent, IExternalCommand, INotifyPropertyChanged {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements) {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Selection selection = uiDoc.Selection;
            Document doc = uiDoc.Document;

            Comment comment = new Comment(uiDoc);

            // Choose object
            try {
                IList<Reference> objRefs = selection.PickObjects(ObjectType.Element, Prompts.SELECT_OBJS);
                foreach (Reference objRef in objRefs) {
                    Element elem = doc.GetElement(objRef);
                    comment.addElement(new ElementModel(elem.Id, elem.Name));
                }
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException) {
                return Result.Cancelled;
            }

            // Choose place for text
            try {
                XYZ commentPoint = selection.PickPoint(Prompts.PLAC_NOTE);
                comment.CommentPosition = commentPoint;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException) {
                return Result.Cancelled;
            }

            // Open comment edit window
            CommentEdit win = new CommentEdit(comment);
            win.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            win.ShowDialog();

            if (win.viewModel.isApply) {
                // Create a comment

                TextNote note;

                using (Transaction trn = new Transaction(doc)) {
                    trn.Start(TransactionNames.CREATE_TEXTNOTE_CUSTOM);

                    note = TextNote.Create(doc, uiDoc.ActiveView.Id, comment.CommentPosition, comment.CommentText,
                                           doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType));

                    comment.TextNoteId = note.Id;

                    trn.Commit();
                }
            }
            else {
                return Result.Cancelled;
            }

            CommentObj = comment;
            return Result.Succeeded;
        }


        private Comment commentObj;
        public override Comment CommentObj {
            get {
                return commentObj;
            }

            set {
                commentObj = value;
                OnPropertyChanged(PropertyNames.COMMENT_OBJ);
            }
        }


        public static event PropertyChangedEventHandler PropertyChangedCustom;
        public event PropertyChangedEventHandler PropertyChanged {
            add {
                PropertyChangedCustom += value;
            }
            remove {
                PropertyChangedCustom -= value;
            }
        }
        public void OnPropertyChanged(string propertyName) {
            PropertyChangedCustom?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public static class PropertyNames {
            public const string COMMENT_OBJ = "CommentObj";
        }


        public static string getLocation() {
            return MethodBase.GetCurrentMethod().ReflectedType.FullName;
        }
    }
}
