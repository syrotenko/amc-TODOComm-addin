using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using System;
using TODOComm.UI;

namespace TODOComm {
    class Main : IExternalApplication {
        public static Main ExternalApp;
        
        private UIControlledApplication application;
        private ChangeTextNoteTextHandler changeTextNoteTextHandler;
        private ExternalEvent changeTextNoteTextEvent;
        private const string ADDIN_NAME = "TODOComm add-in";
        private const string CONTROL_PANEL_NAME = "Control panel";

        public Result OnStartup(UIControlledApplication application) {
            this.application = application;

            application.ControlledApplication.ApplicationInitialized += RegisterDockablePanes;

            buildUI(application);
            createExternalEvents();

            ExternalApp = this;

            return Result.Succeeded;
        }

        
        public Result OnShutdown(UIControlledApplication application) {
            return Result.Succeeded;
        }

        public void registerDocumentChanged (EventHandler<DocumentChangedEventArgs> eventHandler) {
            application.ControlledApplication.DocumentChanged += eventHandler;
        }

        public void changeTextNoteText(Document doc, ElementId textNoteId, string newTextValue) {
            this.changeTextNoteTextHandler.doc = doc;
            this.changeTextNoteTextHandler.textNoteId = textNoteId;
            this.changeTextNoteTextHandler.newTextValue = newTextValue;

            this.changeTextNoteTextEvent.Raise();
        }

        
        private void RegisterDockablePanes(object sender, ApplicationInitializedEventArgs e) {
            UIApplication application = new UIApplication((Application)sender);
            Guid guid = new Guid(Properties.Resource.PAIN_GUID);

            application.RegisterDockablePane(new DockablePaneId(guid), Properties.Resource.PANE_TITLE, new UI.TODOCommPane());
        }

        private void createExternalEvents() {
            this.changeTextNoteTextHandler = new ChangeTextNoteTextHandler();
            this.changeTextNoteTextEvent = ExternalEvent.Create(changeTextNoteTextHandler);
        }

        private void buildUI(UIControlledApplication application) {
            application.CreateRibbonTab(ADDIN_NAME);
            var panel = application.CreateRibbonPanel(ADDIN_NAME, CONTROL_PANEL_NAME);

            panel.AddItem(ElementBuilder.createShowPanelButton());
            panel.AddItem(ElementBuilder.createHidePanelButton());
            panel.AddItem(ElementBuilder.createMakeNoteWithoutObjButton());
            panel.AddItem(ElementBuilder.createMakeNoteSingleObjButton());
            panel.AddItem(ElementBuilder.createMakeNoteMultiObjButton());
            panel.AddItem(ElementBuilder.createMakeNoteSelectedObjButton());
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
}
