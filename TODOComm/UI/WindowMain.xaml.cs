using System.Windows;
using TODOComm.Models;

namespace TODOComm.UI {
    /// <summary>
    /// Interaction logic for WindowMain.xaml
    /// </summary>
    public partial class WindowMain : Window {
        public TODOCommViewModel viewModel;

        public WindowMain(Comment comment) {
            InitializeComponent();

            viewModel = new TODOCommViewModel(comment);
            viewModel.CloseWindowCommand = new ControlWindowCommands() { act = () => this.Close() };

            DataContext = viewModel;
        }
    }
}
