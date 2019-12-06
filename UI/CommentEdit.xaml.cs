using System.Windows;
using TODOComm.Models;
using TODOComm.ViewModel;

namespace TODOComm.UI {
    /// <summary>
    /// Interaction logic for WindowMain.xaml
    /// </summary>
    public partial class CommentEdit : Window {
        public CommentEditViewModel viewModel;

        public CommentEdit(Comment comment) {
            InitializeComponent();

            viewModel = new CommentEditViewModel(comment);
            viewModel.CloseWindowCommand = new CommentEditCommand() { act = () => this.Close() };

            DataContext = viewModel;
        }
    }
}
