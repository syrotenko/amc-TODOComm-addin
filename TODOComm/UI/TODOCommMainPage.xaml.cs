using Autodesk.Revit.UI;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;

namespace TODOComm.UI {
    /// <summary>
    /// Interaction logic for TODOCommMainPage.xaml
    /// </summary>
    public partial class TODOCommMainPage : Page, IDockablePaneProvider {
        TODOCommPaneViewModel viewModel = new TODOCommPaneViewModel();
        public TODOCommMainPage() {
            InitializeComponent();
            DataContext = viewModel;
        }

        public void SetupDockablePane(DockablePaneProviderData data) {
            data.FrameworkElement = this;
            data.InitialState = new DockablePaneState {
                DockPosition = DockPosition.Right
            };
        }

        void lbi_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if (viewModel.EditComm.CanExecute((((ListBox)sender).Tag)))
                viewModel.EditComm.Execute((((ListBox)sender).Tag));
        }
    }
}
