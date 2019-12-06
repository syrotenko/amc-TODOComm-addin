using Autodesk.Revit.UI;
using System.Reflection;
using TODOComm.Commands;

namespace TODOComm.UI {
    public static class ElementBuilder {
        public static PushButtonData createShowPanelButton() {
            PushButtonData pushButtonData = new PushButtonData("showPanel", "Show panel",
                                                               Assembly.GetExecutingAssembly().Location,
                                                               ShowPanelCommand.getLocation());

            pushButtonData.Image = Helper.getBitmapImage(@"\resources\showPanel_16x16.png");
            pushButtonData.LargeImage = Helper.getBitmapImage(@"\resources\showPanel_32x32.png");

            return pushButtonData;
        }

        public static PushButtonData createHidePanelButton() {
            PushButtonData pushButtonData = new PushButtonData("hidePanel", "Hide panel",
                                                               Assembly.GetExecutingAssembly().Location,
                                                               HidePanelCommand.getLocation());

            // TODO: change img
            pushButtonData.Image = Helper.getBitmapImage(@"\resources\showPanel_16x16.png");
            pushButtonData.LargeImage = Helper.getBitmapImage(@"\resources\showPanel_32x32.png");

            return pushButtonData;
        }

        public static PushButtonData createMakeNoteWithoutObjButton() {
            PushButtonData pushButtonData = new PushButtonData("makeNoteWithoutObj", "Make comment\nwithout object",
                                                               Assembly.GetExecutingAssembly().Location,
                                                               MakeNoteWithoutObjCommand.getLocation());

            // TODO: change img
            pushButtonData.Image = Helper.getBitmapImage(@"\resources\showPanel_16x16.png");
            pushButtonData.LargeImage = Helper.getBitmapImage(@"\resources\showPanel_32x32.png");

            return pushButtonData;
        }

        public static RibbonItemData createMakeNoteSingleObjButton() {
            PushButtonData pushButtonData = new PushButtonData("makeNoteSingleObj", "Make comment\nfor single object",
                                                               Assembly.GetExecutingAssembly().Location,
                                                               MakeNoteSingleObjCommand.getLocation());

            // TODO: change img
            pushButtonData.Image = Helper.getBitmapImage(@"\resources\showPanel_16x16.png");
            pushButtonData.LargeImage = Helper.getBitmapImage(@"\resources\showPanel_32x32.png");

            return pushButtonData;
        }

        public static RibbonItemData createMakeNoteMultiObjButton() {
            PushButtonData pushButtonData = new PushButtonData("makeNoteMultiObj", "Make comment\nfor several objects",
                                                               Assembly.GetExecutingAssembly().Location,
                                                               MakeNoteMultiObjCommand.getLocation());

            // TODO: change img
            pushButtonData.Image = Helper.getBitmapImage(@"\resources\showPanel_16x16.png");
            pushButtonData.LargeImage = Helper.getBitmapImage(@"\resources\showPanel_32x32.png");

            return pushButtonData;
        }
    }
}
