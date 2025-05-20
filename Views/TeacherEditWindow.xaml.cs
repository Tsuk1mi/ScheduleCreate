using System.Windows;
using ScheduleCreate.ViewModels;

namespace ScheduleCreate.Views
{
    public partial class TeacherEditWindow : Window
    {
        public TeacherEditWindow(TeacherEditViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.SetWindow(this);
        }
    }
} 