using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using TODOComm.Helper;

namespace TODOComm.Transactions {
    class HideElementsHandler : IExternalEventHandler {
        public Document doc;
        public View view;
        public ICollection<ElementId> elemIds;

        public void Execute(UIApplication uiapp) {

            if (doc != null && (view != null && elemIds != null)) {

                using (var trn = new Transaction(doc, TransactionNames.HIDE_ELEMENTS_CUSTOM)) {
                    trn.Start();

                    view.HideElements(elemIds);

                    trn.Commit();
                }

                doc = null;
                view = null;
                elemIds = null;
            }
        }
        public string GetName() {
            return TransactionNames.HIDE_ELEMENTS_CUSTOM + " event";
        }
    }
}
