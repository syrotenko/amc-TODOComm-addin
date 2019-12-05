using Autodesk.Revit.UI;
using System;

namespace TODOComm.UI {
    class TODOCommPane {
        public static Guid GUID {
            get {
                return new Guid(_GUID);
            }
            private set { }
        }

        private const string _GUID = "BB465D04-71A7-441F-BAFC-485544C61257";
        private const string paneTitle = "TODOComm";

        public void register (UIApplication application) {
            application.RegisterDockablePane(new DockablePaneId(GUID),
                                             paneTitle,
                                             new TODOCommMainPage());
        }
    }
}
