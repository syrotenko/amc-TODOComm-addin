using Autodesk.Revit.DB;
using System.ComponentModel;

namespace TODOComm.Models {
    public class ElementModel : INotifyPropertyChanged {
        public ElementModel (ElementId id, string name, XYZ position) {
            Id = id;
            Name = name;
            Position = position;
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

        private XYZ position;
        public XYZ Position {
            get {
                return position;
            }
            set {
                position = value;

                OnPropertyChanged(PropertyNames.POSITION);
            }
        }


        private Leader leader;
        public Leader Leader {
            get {
                return leader;
            }

            set {
                leader = value;
                OnPropertyChanged(PropertyNames.LEADER);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static class PropertyNames {
            public const string ID = "Id";
            public const string NAME = "Name";
            public const string POSITION = "Position";
            public const string LEADER = "Leader";
        }
    }
}
