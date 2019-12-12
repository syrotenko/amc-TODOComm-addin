using Autodesk.Revit.UI;

namespace TODOComm.Transactions {
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
}
