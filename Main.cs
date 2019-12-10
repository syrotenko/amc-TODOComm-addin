using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using TODOComm.Models;
using TODOComm.UI;

namespace TODOComm {
    class Main : IExternalApplication {
        public Main() {
            todoModel = TODOCommModel.getInstance();
            ExternalApp = this;
            Transactions = new TransactionsAvailable();
        }


        public static Main ExternalApp;
        public TransactionsAvailable Transactions;

        private const string ADDIN_NAME = "TODOComm add-in";
        private const string CONTROL_PANEL_NAME = "Control panel";

        private TODOCommModel todoModel;
        private UIControlledApplication application;

        private ExternalAppEvent createTextNoteHandler;
        private ExternalAppEvent changeTextNoteTextHandler;

        private ExternalAppEvent showElementsHandler;
        private ExternalAppEvent hideElementsHandler;

        private ExternalAppEvent createLeadersHandler;
        private ExternalAppEvent removeLeadersHandler;
        private ExternalAppEvent updateLeadersHandler;


        public Result OnStartup(UIControlledApplication application) {
            this.application = application;

            application.ControlledApplication.ApplicationInitialized += RegisterDockablePanes;

            buildUI(application);

            // Remark: it's necessary to create and register events in OnStartup method
            // because UIControlledApplication instance is garantee created after OnStartup call
            registerEventsAndHandlers();
            createExternalEvents();

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application) {
            return Result.Succeeded;
        }


        public class TransactionsAvailable {
            public Action<Comment> CreateTextNote = Main.ExternalApp.createTextNote;
            public Action<Document, ElementId, string> ChangeTextNoteText = Main.ExternalApp.changeTextNoteText;
            public Action<Document, View, ICollection<ElementId>> ShowElements = Main.ExternalApp.showElements;
            public Action<Document, View, ICollection<ElementId>> HideElements = Main.ExternalApp.hideElements;
            public Action<Document, Dictionary<TextNote, IEnumerable<ElementModel>>> CreateLeaders = Main.ExternalApp.createLeaders;
            public Action<Document, IEnumerable<TextNote>> RemoveLeaders = Main.ExternalApp.removeLeaders;
            public Action<Document, Dictionary<Leader, XYZ>> UpdateLeader = Main.ExternalApp.updateLeaders;
        }


        private void buildUI(UIControlledApplication application) {
            application.CreateRibbonTab(ADDIN_NAME);
            var panel = application.CreateRibbonPanel(ADDIN_NAME, CONTROL_PANEL_NAME);

            panel.AddItem(RevitUI.createShowPanelButton());
            panel.AddItem(RevitUI.createHidePanelButton());
            panel.AddItem(RevitUI.createMakeNoteWithoutObjButton());
            panel.AddItem(RevitUI.createMakeNoteSingleObjButton());
            panel.AddItem(RevitUI.createMakeNoteMultiObjButton());
            panel.AddItem(RevitUI.createMakeNoteSelectedObjButton());
        }

        private void RegisterDockablePanes(object sender, ApplicationInitializedEventArgs e) {
            UIApplication application = new UIApplication((Application)sender);
            Guid guid = new Guid(Properties.Resource.PAIN_GUID);

            application.RegisterDockablePane(new DockablePaneId(guid), Properties.Resource.PANE_TITLE, new UI.TODOCommPane());
        }

        private void registerEventsAndHandlers() {
            registerDocumentChanged(new EventHandler<DocumentChangedEventArgs>(todoModel.wasChangeHandler));
        }


        private void createExternalEvents() {
            this.createTextNoteHandler = new ExternalAppEvent(new CreateTextNoteHandler());
            this.changeTextNoteTextHandler = new ExternalAppEvent(new ChangeTextNoteTextHandler());
            this.showElementsHandler = new ExternalAppEvent(new ShowElementsHandler());
            this.hideElementsHandler = new ExternalAppEvent(new HideElementsHandler());
            this.createLeadersHandler = new ExternalAppEvent(new CreateLeadersHandler());
            this.removeLeadersHandler = new ExternalAppEvent(new RemoveLeadersHandler());
            this.updateLeadersHandler = new ExternalAppEvent(new UpdateLeaderHandler());
        }

        private void createTextNote(Comment comment) {
            ((CreateTextNoteHandler)createTextNoteHandler.handler).comment = comment;

            this.createTextNoteHandler.Raise();
        }

        private void changeTextNoteText(Document doc, ElementId textNoteId, string newTextValue) {
            ChangeTextNoteTextHandler handler = (ChangeTextNoteTextHandler)changeTextNoteTextHandler.handler;
            handler.doc = doc;
            handler.textNoteId = textNoteId;
            handler.newTextValue = newTextValue;

            this.changeTextNoteTextHandler.Raise();
        }

        private void showElements(Document doc, View view, ICollection<ElementId> elementIds) {
            ShowElementsHandler handler = (ShowElementsHandler)showElementsHandler.handler;

            handler.doc = doc;
            handler.view = view;
            handler.elemIds = elementIds;

            this.showElementsHandler.Raise();
        }

        private void hideElements(Document doc, View view, ICollection<ElementId> elementIds) {
            HideElementsHandler handler = (HideElementsHandler)hideElementsHandler.handler;

            handler.doc = doc;
            handler.view = view;
            handler.elemIds = elementIds;

            this.hideElementsHandler.Raise();
        }
        private void createLeaders(Document doc, Dictionary<TextNote, IEnumerable<ElementModel>> updateInfo) {
            CreateLeadersHandler handler = (CreateLeadersHandler)createLeadersHandler.handler;

            handler.doc = doc;
            handler.updateInfo = updateInfo;

            this.createLeadersHandler.Raise();
        }
        private void removeLeaders(Document doc, IEnumerable<TextNote> textNotes) {
            RemoveLeadersHandler handler = (RemoveLeadersHandler)removeLeadersHandler.handler;

            handler.doc = doc;
            handler.textNotes = textNotes;

            this.removeLeadersHandler.Raise();
        }
        private void updateLeaders(Document doc, Dictionary<Leader, XYZ> updateInfo) {
            UpdateLeaderHandler handler = (UpdateLeaderHandler)updateLeadersHandler.handler;

            handler.doc = doc;
            handler.updateInfo = updateInfo;

            this.updateLeadersHandler.Raise();
        }


        private void registerDocumentChanged(EventHandler<DocumentChangedEventArgs> eventHandler) {
            application.ControlledApplication.DocumentChanged += eventHandler;
        }
    }

    struct ExternalAppEvent {
        public ExternalAppEvent(IExternalEventHandler handler) {
            this.handler = handler;
            handlerEvent = ExternalEvent.Create(this.handler);
        }

        public IExternalEventHandler handler;
        public ExternalEvent handlerEvent;

        public void Raise() {
            handlerEvent.Raise();
        }
    }


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

    class CreateTextNoteHandler : IExternalEventHandler {
        public Comment comment;

        public void Execute(UIApplication uiapp) {

            if (comment != null) {

                using (Transaction trn = new Transaction(comment.doc)) {
                    trn.Start(TransactionNames.CREATE_TEXTNOTE_CUSTOM);

                    TextNote note = TextNote.Create(comment.doc, comment.uiDoc.ActiveView.Id, comment.CommentPosition, comment.CommentText,
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
