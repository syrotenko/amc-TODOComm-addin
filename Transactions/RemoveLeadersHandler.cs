using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using TODOComm.Helper;

namespace TODOComm.Transactions {
    class RemoveLeadersHandler : IExternalEventHandler {
        public Document doc;
        public IEnumerable<TextNote> textNotes;

        public void Execute(UIApplication uiapp) {
            if (doc != null && textNotes != null) {

                using (Transaction trn = new Transaction(doc)) {
                    trn.Start(TransactionNames.REMOVE_LEADERS_CUSTOM);

                    foreach (TextNote textNote in textNotes) {
                        textNote.RemoveLeaders();
                    }

                    trn.Commit();
                }
            }
        }

        public string GetName() {
            return TransactionNames.REMOVE_LEADERS_CUSTOM + " event";
        }
    }
}
