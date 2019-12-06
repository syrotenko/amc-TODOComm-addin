using Autodesk.Revit.DB;
using System.ComponentModel;

namespace TODOComm.Models {
    public class ElementModel : INotifyPropertyChanged {
        public ElementModel (ElementId id, string name) {
            Id = id;
            Name = name;
        }


        private ElementId id;
        public ElementId Id {
            get {
                return id;
            }
            set {
                id = value;
                OnPropertyChanged(PropertyNames.ID);
            }
        }

        private string name;
        public string Name {
            get {
                return name;
            }
            set {
                name = value;
                OnPropertyChanged(PropertyNames.NAME);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static class PropertyNames {
            public const string ID = "Id";
            public const string NAME = "Name";
        }
    }
}
