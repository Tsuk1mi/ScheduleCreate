using System.Windows;
using ScheduleCreate.ViewModels;

namespace ScheduleCreate.Views
{
    public partial class TeacherManagementWindow : Window
    {
        public TeacherManagementWindow(TeacherManagementViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.SetWindow(this);
        }
    }
} 