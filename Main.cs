﻿using Autodesk.Revit.ApplicationServices;
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
        
        private ExternalAppEvent createLeaderHandler;
        private ExternalAppEvent removeLeadersHandler;
        private ExternalAppEvent updateLeaderHandler;


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
            public Action<Document, TextNote, IEnumerable<ElementModel>> CreateLeader = Main.ExternalApp.createLeader;
            public Action<Document, TextNote> RemoveLeaders = Main.ExternalApp.removeLeaders;
            public Action<Document, Dictionary<Leader, XYZ>> UpdateLeader = Main.ExternalApp.updateLeader;
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
            this.createLeaderHandler = new ExternalAppEvent(new CreateLeadersHandler());
            this.removeLeadersHandler = new ExternalAppEvent(new RemoveLeadersHandler());
            this.updateLeaderHandler = new ExternalAppEvent(new UpdateLeaderHandler());
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
            handler.elementIds = elementIds;

            this.showElementsHandler.Raise();
        }

        private void hideElements(Document doc, View view, ICollection<ElementId> elementIds) {
            HideElementsHandler handler = (HideElementsHandler)hideElementsHandler.handler;

            handler.doc = doc;
            handler.view = view;
            handler.elementIds = elementIds;

            this.hideElementsHandler.Raise();
        }
        private void createLeader(Document doc, TextNote textNote, IEnumerable<ElementModel> elements) {
            CreateLeadersHandler handler = (CreateLeadersHandler)createLeaderHandler.handler;

            handler.doc = doc;
            handler.textNote = textNote;
            handler.elements = elements;

            this.createLeaderHandler.Raise();
        }
        private void removeLeaders(Document doc, TextNote textNote) {
            RemoveLeadersHandler handler = (RemoveLeadersHandler)removeLeadersHandler.handler;

            handler.doc = doc;
            handler.textNote = textNote;

            this.removeLeadersHandler.Raise();
        }
        private void updateLeader(Document doc, Dictionary<Leader, XYZ> updateInfo) {
            UpdateLeaderHandler handler = (UpdateLeaderHandler)updateLeaderHandler.handler;

            handler.doc= doc;
            handler.updateInfo = updateInfo;

            this.updateLeaderHandler.Raise();
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

    class CreateLeadersHandler : IExternalEventHandler {
        public Document doc;
        public TextNote textNote;
        public IEnumerable<ElementModel> elements;

        public void Execute(UIApplication uiapp) {
            if (doc != null) {

                using (Transaction trn = new Transaction(doc)) {
                    trn.Start(TransactionNames.CREATE_TEXTNOTE_CUSTOM);

                    foreach (var element in elements) {
                        element.Leader = textNote.AddLeader(TextNoteLeaderTypes.TNLT_STRAIGHT_L);
                        element.Leader.End = element.Position;
                    }

                    trn.Commit();
                }
            }
        }

        public string GetName() {
            // TODO: change name of event
            return TransactionNames.EDIT_TEXT_CUSTOM + " event";
        }
    }

    class RemoveLeadersHandler : IExternalEventHandler {
        public Document doc;
        public TextNote textNote;

        public void Execute(UIApplication uiapp) {
            if (doc != null) {
                using (Transaction trn = new Transaction(doc)) {
                    trn.Start(TransactionNames.CREATE_TEXTNOTE_CUSTOM);

                    textNote.RemoveLeaders();

                    trn.Commit();
                }
            }
        }

        public string GetName() {
            // TODO: change name of event
            return TransactionNames.EDIT_TEXT_CUSTOM + " event";
        }
    }

    class UpdateLeaderHandler : IExternalEventHandler {
        public Document doc;
        public Dictionary<Leader, XYZ> updateInfo;

        public void Execute(UIApplication uiapp) {
            if (doc != null) {
                TextNote note;

                using (Transaction trn = new Transaction(doc)) {
                    trn.Start(TransactionNames.CREATE_TEXTNOTE_CUSTOM);

                    foreach (KeyValuePair<Leader, XYZ> entry in updateInfo) {
                        entry.Key.End = entry.Value;
                    }

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
