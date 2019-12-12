using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using TODOComm.Models;
using TODOComm.Helper;

namespace TODOComm.Transactions {
    class CreateTextNoteHandler : IExternalEventHandler {
        public Comment comment;

        public void Execute(UIApplication uiapp) {

            if (comment != null) {

                using (Transaction trn = new Transaction(comment.doc)) {
                    trn.Start(TransactionNames.CREATE_TEXTNOTE_CUSTOM);

                    TextNote note = TextNote.Create(comment.doc, comment.view.Id, comment.CommentPosition, comment.CommentText,
                                                    comment.doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType));

                    comment.TextNoteId = note.Id;

                    trn.Commit();
                }
            }
        }

        public string GetName() {
            return TransactionNames.CREATE_TEXTNOTE_CUSTOM + " event";
        }
    }
}
