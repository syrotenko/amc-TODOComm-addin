using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Diagnostics;
using TODOComm.UI;

namespace TODOComm {
    class Main : IExternalApplication {
        public ExternalEventMy handler_event;
        public ExternalEvent exEvent;
        public UIControlledApplication application;

        public static Main thisApp;

        private const string addinName = "TODOComm add-in";

        public Result OnStartup(UIControlledApplication application) {
            Debug.WriteLine("Startup app");

            this.application = application;

            buildUI(application);
            application.ControlledApplication.ApplicationInitialized += RegisterDockablePanes;

            this.handler_event = new ExternalEventMy();
            this.exEvent = ExternalEvent.Create(handler_event);

            //application.ControlledApplication.DocumentChanged += new EventHandler<Autodesk.Revit.DB.Events.DocumentChangedEventArgs>(wasChange);

            thisApp = this;

            return Result.Succeeded;
        }

        //public void wasChange(object sender, DocumentChangedEventArgs args) {
        //    args.GetModifiedElementIds();
        //    Debug.WriteLine("OWN: AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
        //}

        public void changeTextNoteText(Document doc, ElementId textNoteId, string newTextValue) {
            this.handler_event.doc = doc;
            this.handler_event.textNoteId = textNoteId;
            this.handler_event.newTextValue = newTextValue;

            this.exEvent.Raise();
        }

        public Result OnShutdown(UIControlledApplication application) {
            Debug.WriteLine("Close app");

            return Result.Succeeded;
        }

        private void RegisterDockablePanes(object sender, Autodesk.Revit.DB.Events.ApplicationInitializedEventArgs e) {
            TODOCommPane pane = new TODOCommPane();
            pane.register(new UIApplication((Application)sender));
        }

        private void buildUI(UIControlledApplication application) {
            application.CreateRibbonTab(addinName);
            var panel = application.CreateRibbonPanel(addinName, "Control panel");

            panel.AddItem(ElementBuilder.createShowPanelButton());
            panel.AddItem(ElementBuilder.createHidePanelButton());
            panel.AddItem(ElementBuilder.createMakeNoteSingleObjButton());
            panel.AddItem(ElementBuilder.createMakeNoteMultiObjButton());
        }
    }

    class ExternalEventMy : IExternalEventHandler {
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

    class ExternalEventMy2 : IExternalEventHandler {
        public void Execute(UIApplication app) {
            throw new System.NotImplementedException();
        }

        public string GetName() {
            throw new System.NotImplementedException();
        }
    }
}
