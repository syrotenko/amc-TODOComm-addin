using Autodesk.Revit.DB;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;

namespace TODOComm {
    public static class Helper {
        public static BitmapImage getBitmapImage(string path) {
            return new BitmapImage(new Uri(getFullPath(path)));
        }

        public static string getFullPath(string path) {
            return Directory.GetCurrentDirectory() + path;
        }

        public static XYZ GetElementLocation(Element elem) {
            Location loc = elem.Location;

            LocationPoint lp = loc as LocationPoint;
            if (null != lp) {
                return lp.Point;
            }
            else {
                LocationCurve lc = loc as LocationCurve;

                Debug.WriteLine("expected location to be either point or curve");
                    
                return lc.Curve.GetEndPoint(0);
            }
        }
    }

    static class Prompts {
        public const string SELECT_OBJ = "Select object";
        public const string SELECT_OBJS = "Select several objects";
        public const string PLAC_NOTE = "Place comment here";
    }

    static class TransactionNames {
        public const string EDIT_TEXT = "Edit Text";
        public const string EDIT_TEXT_CUSTOM = "Edit Text_TODOComm";
        public const string CREATE_TEXTNOTE_CUSTOM = "Create TextNote_TODOComm";
    }
}
