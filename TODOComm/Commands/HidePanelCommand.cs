using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using TODOComm.UI;

namespace TODOComm.Commands {
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    class HidePanelCommand : IExternalCommand {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements) {
            DockablePane pane = commandData.Application.GetDockablePane(new DockablePaneId(TODOCommPane.GUID));
            pane.Hide();

            return Result.Succeeded;
        }

        public static string getLocation() {
            return typeof(HidePanelCommand).FullName;
        }
    }
}
