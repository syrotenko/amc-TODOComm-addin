using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using TODOComm.Models;
using TODOComm.UI;

namespace TODOComm.Commands {
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    class MakeNoteSingleObjCommand : IExternalCommand {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements) {
            Selection selection = commandData.Application.ActiveUIDocument.Selection;

            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;
            TextNote note;

            Comment comm = new Comment(uiDoc);

            // Choose object
            try {
                Reference objRef = selection.PickObject(ObjectType.Element, Prompts.SELECT_OBJ);
                Element elem = doc.GetElement(objRef);

                comm.addElement(new ElementModel(elem.Id, elem.Name));
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException) {
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
            return typeof(MakeNoteSingleObjCommand).FullName;
        }
    }
}
