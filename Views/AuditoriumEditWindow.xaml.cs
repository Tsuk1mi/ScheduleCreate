using System.Windows;
using ScheduleCreate.ViewModels;

namespace ScheduleCreate.Views
{
    public partial class AuditoriumEditWindow : Window
    {
        public AuditoriumEditWindow(AuditoriumEditViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.SetWindow(this);
        }
    }
} 