using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using TODOComm.Models;
using TODOComm.Helper;

namespace TODOComm.Transactions {
    class CreateLeadersHandler : IExternalEventHandler {
        public Document doc;
        public Dictionary<TextNote, IEnumerable<ElementModel>> updateInfo;

        public void Execute(UIApplication uiapp) {
            if (doc != null && updateInfo != null) {

                using (Transaction trn = new Transaction(doc)) {
                    trn.Start(TransactionNames.CREATE_LEADERS_CUSTOM + Guid.NewGuid().ToString());

                    foreach (KeyValuePair<TextNote, IEnumerable<ElementModel>> entry in updateInfo) {
                        TextNote textNote = entry.Key;
                        IEnumerable<ElementModel> elems = entry.Value;

                        foreach (var element in elems) {
                            element.Leader = textNote.AddLeader(TextNoteLeaderTypes.TNLT_STRAIGHT_L);
                            element.Leader.End = element.Position;
                        }
                    }

                    trn.Commit();
                }
            }
        }

        public string GetName() {
            return TransactionNames.CREATE_LEADERS_CUSTOM + " event";
        }
    }
}
