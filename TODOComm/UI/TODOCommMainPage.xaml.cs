using Autodesk.Revit.UI;
using System.Windows.Controls;
using System.Windows.Input;

namespace TODOComm.UI {
    /// <summary>
    /// Interaction logic for TODOCommMainPage.xaml
    /// </summary>
    public partial class TODOCommMainPage : Page, IDockablePaneProvider {
        TODOCommPaneViewModel viewModel;
        public TODOCommMainPage() {
            viewModel = new TODOCommPaneViewModel();
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
            if (viewModel.EditComm.CanExecute(((ListBoxItem)sender).DataContext))
                viewModel.EditComm.Execute(((ListBoxItem)sender).DataContext);
        }

        void lbi_MouseDown(object sender, MouseEventArgs e) {
            if (viewModel.SelectComm.CanExecute(((ListBoxItem)sender).DataContext))
                viewModel.SelectComm.Execute(((ListBoxItem)sender).DataContext);
        }
    }
}
