using System.Windows;
using ScheduleCreate.ViewModels;

namespace ScheduleCreate.Views
{
    public partial class GroupEditWindow : Window
    {
        public GroupEditWindow(GroupEditViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.SetWindow(this);
        }
    }
} 