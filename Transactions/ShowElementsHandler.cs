using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;

namespace TODOComm.Transactions {
    class ShowElementsHandler : IExternalEventHandler {
        public Document doc;
        public View view;
        public ICollection<ElementId> elemIds;

        public void Execute(UIApplication uiapp) {

            if (doc != null && (view != null && elemIds != null)) {

                using (var trn = new Transaction(doc, TransactionNames.SHOW_ELEMENTS_CUSTOM)) {
                    trn.Start();

                    view.UnhideElements(elemIds);

                    trn.Commit();
                }

                doc = null;
                view = null;
                elemIds = null;
            }
        }
        public string GetName() {
            return TransactionNames.SHOW_ELEMENTS_CUSTOM + " event";
        }
    }
}
