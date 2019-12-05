using System;
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
    }

    static class Prompts {
        public const string SELECT_OBJ = "Select object";
        public const string SELECT_OBJS = "Select several objects";
        public const string PLAC_NOTE = "Place comment here";
    }

    static class TransactionNames {
        public const string EDIT_TEXT = "Edit Text";
        public const string EDIT_TEXT_CUSTOM = "Edit Text_TODOComm";
        public static string CREATE_TEXTNOTE_CUSTOM = "Create TextNote_TODOComm";
    }
}
