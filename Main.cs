using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using TODOComm.Models;
using TODOComm.Transactions;
using TODOComm.UI;

namespace TODOComm {
    class Main : IExternalApplication {
        public Main() {
            todoModel = TODOCommModel.getInstance();
            instance = this;
            Transactions = new TransactionsAvailable();
        }

        private static Main instance;

        public static Main getInstance() {
            if (instance == null)
                instance = new Main();
            return instance;
        }


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
            registerEventHandlers();
            createExternalEvents();

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application) {
            return Result.Succeeded;
        }


        public class TransactionsAvailable {
            public Action<Comment> CreateTextNote = Main.getInstance().createTextNote;
            public Action<Document, ElementId, string> ChangeTextNoteText = Main.getInstance().changeTextNoteText;
            public Action<Document, View, ICollection<ElementId>> ShowElements = Main.getInstance().showElements;
            public Action<Document, View, ICollection<ElementId>> HideElements = Main.getInstance().hideElements;
            public Action<Document, Dictionary<TextNote, IEnumerable<ElementModel>>> CreateLeaders = Main.getInstance().createLeaders;
            public Action<Document, IEnumerable<TextNote>> RemoveLeaders = Main.getInstance().removeLeaders;
            public Action<Document, Dictionary<Leader, XYZ>> UpdateLeader = Main.getInstance().updateLeaders;
        }


        private void buildUI(UIControlledApplication application) {
            application.CreateRibbonTab(ADDIN_NAME);
            RibbonPanel panel = application.CreateRibbonPanel(ADDIN_NAME, CONTROL_PANEL_NAME);

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

        private void registerEventHandlers() {
            registerDocumentChanged(new EventHandler<DocumentChangedEventArgs>(todoModel.wasChangeHandler));
        }

        private void registerDocumentChanged(EventHandler<DocumentChangedEventArgs> eventHandler) {
            application.ControlledApplication.DocumentChanged += eventHandler;
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
    }
}
