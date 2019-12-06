using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Reflection;

namespace TODOComm.Commands {
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    class HidePanelCommand : IExternalCommand {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements) {
            commandData.Application.GetDockablePane(new DockablePaneId(new Guid(Properties.Resource.PAIN_GUID)))
                .Hide();

            return Result.Succeeded;
        }

        public static string getLocation() {
            return MethodBase.GetCurrentMethod().ReflectedType.FullName;
        }
    }
}
