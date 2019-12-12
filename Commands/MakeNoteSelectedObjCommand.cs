using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Reflection;
using TODOComm.Models;
using TODOComm.UI;
using System.Linq;
using TODOComm.Helper;

namespace TODOComm.Commands {
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    class MakeNoteSelectedObjCommand : IExternalCommand{

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements) {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Selection selection = uiDoc.Selection;
            Document doc = uiDoc.Document;

            Comment comment = new Comment(uiDoc);

            // Get selected elements
            ICollection<ElementId> selectedIds = uiDoc.Selection.GetElementIds();

            if (selectedIds.Count != 0) {
                IEnumerable<Element> elements_ = selectedIds.Select(selectedId => doc.GetElement(selectedId));
                IEnumerable<ElementModel> elementModels = elements_.Select(element => new ElementModel(element.Id, element.Name, HelperClass.GetElementPosition(element)));

                comment.addElements(elementModels);
            }
            else {
                TaskDialog.Show("Create comment for selected objects", "Firstly, select elements.");
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

            return Result.Succeeded;
        }


        public static string getLocation() {
            return MethodBase.GetCurrentMethod().ReflectedType.FullName;
        }
    }
}
