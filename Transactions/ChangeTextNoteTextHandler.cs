using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace TODOComm.Transactions {
    class ChangeTextNoteTextHandler : IExternalEventHandler {
        public Document doc;
        public ElementId textNoteId;
        public string newTextValue;

        public void Execute(UIApplication uiapp) {

            if (doc != null && (!string.IsNullOrEmpty(newTextValue) && textNoteId != null)) {

                using (Transaction trn = new Transaction(doc)) {
                    trn.Start(TransactionNames.EDIT_TEXT_CUSTOM);

                    ((TextNote)doc.GetElement(this.textNoteId)).Text = newTextValue;

                    trn.Commit();
                }

                doc = null;
                textNoteId = null;
                newTextValue = string.Empty;
            }
        }
        public string GetName() {
            return TransactionNames.EDIT_TEXT_CUSTOM + " event";
        }
    }
}
