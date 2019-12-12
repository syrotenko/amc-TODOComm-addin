using System.Windows;
using TODOComm.Models;
using TODOComm.ViewModel;
using TODOComm.Helper;

namespace TODOComm.UI {
    public partial class CommentEdit : Window {
        public CommentEdit(Comment comment) {
            InitializeComponent();

            CommentEditViewModel viewModel = new CommentEditViewModel(comment);
            viewModel.CloseWindowCommand = new DelegateCommand() { act = (dummy) => this.Close() };

            DataContext = viewModel;
        }
    }
}
