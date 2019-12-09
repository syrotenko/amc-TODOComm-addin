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

        private void createExternalEvents() {
            this.createTextNoteHandler = new ExternalAppEvent(new CreateTextNoteHandler());
            this.changeTextNoteTextHandler = new ExternalAppEvent(new ChangeTextNoteTextHandler());
            this.showElementsHandler = new ExternalAppEvent(new ShowElementsHandler());
            this.hideElementsHandler = new ExternalAppEvent(new HideElementsHandler());
        }

        private void registerEventsAndHandlers() {
            registerDocumentChanged(new EventHandler<DocumentChangedEventArgs>(todoModel.wasChangeHandler));
        }


        private void createTextNote(Comment comment) {
            ((CreateTextNoteHandler)createTextNoteHandler.handler).comment = comment;

            this.createTextNoteHandler.Raise();
        }

        private void changeTextNoteText(Document doc, ElementId textNoteId, string newTextValue) {
            ((ChangeTextNoteTextHandler)changeTextNoteTextHandler.handler).doc = doc;
            ((ChangeTextNoteTextHandler)changeTextNoteTextHandler.handler).textNoteId = textNoteId;
            ((ChangeTextNoteTextHandler)changeTextNoteTextHandler.handler).newTextValue = newTextValue;

            this.changeTextNoteTextHandler.Raise();
        }

        private void showElements(Document doc, View view, ICollection<ElementId> elementIds) {
            ((ShowElementsHandler)showElementsHandler.handler).doc = doc;
            ((ShowElementsHandler)showElementsHandler.handler).view = view;
            ((ShowElementsHandler)showElementsHandler.handler).elementIds = elementIds;

            this.showElementsHandler.Raise();
        }

        private void hideElements(Document doc, View view, ICollection<ElementId> elementIds) {
            ((HideElementsHandler)hideElementsHandler.handler).doc = doc;
            ((HideElementsHandler)hideElementsHandler.handler).view = view;
            ((HideElementsHandler)hideElementsHandler.handler).elementIds = elementIds;

            this.hideElementsHandler.Raise();
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
        public ICollection<ElementId> elementIds;

        public void Execute(UIApplication uiapp) {
            // TODO: check and textNoteId
            if (doc != null) {
                using (var tran = new Transaction(doc, "Test")) {
                    tran.Start();

                    view.UnhideElements(elementIds);

                    tran.Commit();
                }

                doc = null;
                view = null;
                elementIds = null;
            }
        }
        public string GetName() {
            return TransactionNames.EDIT_TEXT_CUSTOM + " event";
        }
    }

    class HideElementsHandler : IExternalEventHandler {
        public Document doc;
        public View view;
        public ICollection<ElementId> elementIds;

        public void Execute(UIApplication uiapp) {
            // TODO: check and textNoteId
            if (doc != null) {
                using (var tran = new Transaction(doc, "Test")) {
                    tran.Start();

                    view.HideElements(elementIds);

                    tran.Commit();
                }

                doc = null;
                view = null;
                elementIds = null;
            }
        }
        public string GetName() {
            return TransactionNames.EDIT_TEXT_CUSTOM + " event";
        }
    }

    class ChangeTextNoteTextHandler : IExternalEventHandler {
        public Document doc;
        public ElementId textNoteId;
        public string newTextValue;

        public void Execute(UIApplication uiapp) {
            // TODO: check and textNoteId
            if (doc != null && !string.IsNullOrEmpty(newTextValue)) {
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
            // TODO: check and textNoteId
            if (comment.doc != null) {
                TextNote note;

                using (Transaction trn = new Transaction(comment.doc)) {
                    trn.Start(TransactionNames.CREATE_TEXTNOTE_CUSTOM);

                    note = TextNote.Create(comment.doc, comment.uiDoc.ActiveView.Id, comment.CommentPosition, comment.CommentText,
                                           comment.doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType));

                    comment.TextNoteId = note.Id;

                    trn.Commit();
                }
            }
        }

        public string GetName() {
            // TODO: change name of event
            return TransactionNames.EDIT_TEXT_CUSTOM + " event";
        }
    }
}
