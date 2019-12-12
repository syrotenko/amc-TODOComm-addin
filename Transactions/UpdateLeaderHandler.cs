using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using TODOComm.Helper;

namespace TODOComm.Transactions {
    class UpdateLeaderHandler : IExternalEventHandler {
        public Document doc;
        public Dictionary<Leader, XYZ> updateInfo;

        public void Execute(UIApplication uiapp) {
            if (doc != null && updateInfo != null) {

                using (Transaction trn = new Transaction(doc)) {
                    trn.Start(TransactionNames.UPDATE_LEADERS_CUSTOM);

                    foreach (KeyValuePair<Leader, XYZ> entry in updateInfo) {

                        /*
                            It's necessary to catch exception because transaction will not be complete if any errors are acquired.
                            Some errors which Revit throws are not errors in fact.
                        
                            Example:
                            0.Comment try..catch code
                            1.Create comment_1 with single object
                            2.Create comment_2 with another single object
                            3.Select object of comment_1 and object of comment_2 and TextNote of comment_2
                            4.Move it
                         
                            You will see, that object of comment_1 is not moved.
                            It happens because when system tries to update Leader of comment_2 object the error appears
                            Revit throws error because when TextNote is moved, Revit creates new Leaders
                            Because of that error, normal update transactions "undo"
                        */
                        try {
                            entry.Key.End = entry.Value;
                        }
                        catch (Autodesk.Revit.Exceptions.InvalidObjectException) { }
                    }

                    trn.Commit();
                }
            }
        }

        public string GetName() {
            return TransactionNames.UPDATE_LEADERS_CUSTOM + " event";
        }
    }
}
