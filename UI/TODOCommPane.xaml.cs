using Autodesk.Revit.UI;
using System.Windows.Controls;
using System.Windows.Input;
using TODOComm.ViewModel;

namespace TODOComm.UI {
    /// <summary>
    /// Interaction logic for TODOCommMainPage.xaml
    /// </summary>
    public partial class TODOCommPane : Page, IDockablePaneProvider {
        public TODOCommPane() {
            InitializeComponent();
            
            viewModel = new TODOCommPaneViewModel();
            DataContext = viewModel;
        }

        TODOCommPaneViewModel viewModel;

        public void SetupDockablePane(DockablePaneProviderData data) {
            data.FrameworkElement = this;
        }

        // TODO: write why such approuch is used
        void MouseDoubleClickEvent(object sender, MouseButtonEventArgs e) {
            if (viewModel.EditComm.CanExecute(((ListBoxItem)sender).DataContext))
                viewModel.EditComm.Execute(((ListBoxItem)sender).DataContext);
        }

        void MouseEnterEvent_(object sender, MouseEventArgs e) {
            if (viewModel.SelectComm.CanExecute(((ListBoxItem)sender).DataContext))
                viewModel.SelectComm.Execute(((ListBoxItem)sender).DataContext);
        }
    }
}
