using System.Windows;
using ScheduleCreate.ViewModels;

namespace ScheduleCreate.Views
{
    public partial class ScheduleEntryWindow : Window
    {
        public ScheduleEntryWindow(ScheduleEntryViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.SetWindow(this);
        }
    }
} 