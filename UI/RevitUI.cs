using Autodesk.Revit.UI;
using System.Reflection;
using TODOComm.Commands;
using TODOComm.Helper;

namespace TODOComm.UI {
    public static class RevitUI {
        public static PushButtonData createShowPanelButton() {
            PushButtonData pushButtonData = new PushButtonData("showPanel", "Show panel",
                                                               Assembly.GetExecutingAssembly().Location,
                                                               ShowPanelCommand.getLocation());

            pushButtonData.Image = HelperClass.getBitmapImage(@"\resources\showPanel_16x16.png");
            pushButtonData.LargeImage = HelperClass.getBitmapImage(@"\resources\showPanel_32x32.png");

            return pushButtonData;
        }

        public static PushButtonData createHidePanelButton() {
            PushButtonData pushButtonData = new PushButtonData("hidePanel", "Hide panel",
                                                               Assembly.GetExecutingAssembly().Location,
                                                               HidePanelCommand.getLocation());

            // TODO: change img
            pushButtonData.Image = HelperClass.getBitmapImage(@"\resources\showPanel_16x16.png");
            pushButtonData.LargeImage = HelperClass.getBitmapImage(@"\resources\hidePanel_32x32.png");

            return pushButtonData;
        }

        public static PushButtonData createMakeNoteWithoutObjButton() {
            PushButtonData pushButtonData = new PushButtonData("makeNoteWithoutObj", "Make comment\nwithout object",
                                                               Assembly.GetExecutingAssembly().Location,
                                                               MakeNoteWithoutObjCommand.getLocation());

            // TODO: change img
            pushButtonData.Image = HelperClass.getBitmapImage(@"\resources\showPanel_16x16.png");
            pushButtonData.LargeImage = HelperClass.getBitmapImage(@"\resources\withoutObj_32x32.png");

            return pushButtonData;
        }

        public static RibbonItemData createMakeNoteSingleObjButton() {
            PushButtonData pushButtonData = new PushButtonData("makeNoteSingleObj", "Make comment\nfor single object",
                                                               Assembly.GetExecutingAssembly().Location,
                                                               MakeNoteSingleObjCommand.getLocation());

            // TODO: change img
            pushButtonData.Image = HelperClass.getBitmapImage(@"\resources\showPanel_16x16.png");
            pushButtonData.LargeImage = HelperClass.getBitmapImage(@"\resources\singleObj_32x32.png");

            return pushButtonData;
        }

        public static RibbonItemData createMakeNoteMultiObjButton() {
            PushButtonData pushButtonData = new PushButtonData("makeNoteMultiObj", "Make comment\nfor several objects",
                                                               Assembly.GetExecutingAssembly().Location,
                                                               MakeNoteMultiObjCommand.getLocation());

            // TODO: change img
            pushButtonData.Image = HelperClass.getBitmapImage(@"\resources\showPanel_16x16.png");
            pushButtonData.LargeImage = HelperClass.getBitmapImage(@"\resources\multiObj_32x32.png");

            return pushButtonData;
        }

        public static RibbonItemData createMakeNoteSelectedObjButton() {
            PushButtonData pushButtonData = new PushButtonData("makeNoteSelectedObj", "Make comment\nfor selected objects",
                                                               Assembly.GetExecutingAssembly().Location,
                                                               MakeNoteSelectedObjCommand.getLocation());

            // TODO: change img
            pushButtonData.Image = HelperClass.getBitmapImage(@"\resources\showPanel_16x16.png");
            pushButtonData.LargeImage = HelperClass.getBitmapImage(@"\resources\selectedObj_32x32.png");

            return pushButtonData;
        }
    }
}
