using System.Windows;
using TODOComm.Models;

namespace TODOComm.UI {
    /// <summary>
    /// Interaction logic for WindowMain.xaml
    /// </summary>
    public partial class WindowMain : Window {
        public CommentEditViewModel viewModel;

        public WindowMain(Comment comment) {
            InitializeComponent();

            viewModel = new CommentEditViewModel(comment);
            viewModel.CloseWindowCommand = new CommentEditCommand() { act = () => this.Close() };

            DataContext = viewModel;
        }
    }
}
