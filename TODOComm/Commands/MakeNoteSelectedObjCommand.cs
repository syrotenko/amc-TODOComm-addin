﻿using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Reflection;
using TODOComm.Models;
using TODOComm.UI;

namespace TODOComm.Commands {
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    class MakeNoteSelectedObjCommand : IExternalCommand {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements) {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Selection selection = uiDoc.Selection;
            Document doc = uiDoc.Document;

            Comment comm = new Comment(uiDoc);

            // Get selected elements
            ICollection<ElementId> selectedIds = uiDoc.Selection.GetElementIds();

            if (selectedIds.Count != 0) {
                foreach (ElementId elementId in selectedIds) {
                    Element elem = doc.GetElement(elementId);
                    comm.addElement(new ElementModel(elem.Id, elem.Name));
                }
            }
            else {
                TaskDialog.Show("Create comment for selected objects", "Firstly, select elements.");
                return Result.Cancelled;
            }

            // Choose place for text
            try {
                XYZ commentPoint = selection.PickPoint(Prompts.PLAC_NOTE);
                comm.CommentPosition = commentPoint;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException) {
                return Result.Cancelled;
            }

            // Open comment edit window
            WindowMain win = new WindowMain(comm);
            win.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            win.ShowDialog();

            if (win.viewModel.isApply) {
                // Create a comment

                TextNote note;

                using (Transaction trn = new Transaction(doc)) {
                    trn.Start(TransactionNames.CREATE_TEXTNOTE_CUSTOM);

                    note = TextNote.Create(doc, uiDoc.ActiveView.Id, comm.CommentPosition, comm.CommentText,
                                           doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType));

                    comm.TextNoteId = note.Id;

                    trn.Commit();
                }

                TODOCommModel.getInstance().addComment(comm);
            }
            else {
                return Result.Cancelled;
            }

            return Result.Succeeded;
        }

        public static string getLocation() {
            return MethodBase.GetCurrentMethod().ReflectedType.FullName;
        }
    }
}
