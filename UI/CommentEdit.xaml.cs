using System.Windows;
using TODOComm.Models;
using TODOComm.ViewModel;

namespace TODOComm.UI {
    public partial class CommentEdit : Window {
        public CommentEdit(Comment comment) {
            InitializeComponent();

            CommentEditViewModel viewModel = new CommentEditViewModel(comment);
            viewModel.CloseWindowCommand = new CommentEditCommand() { act = () => this.Close() };

            DataContext = viewModel;
        }
    }
}
