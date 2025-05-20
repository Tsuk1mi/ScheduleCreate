using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ScheduleCreate.Models;
using ScheduleCreate.Services;
using ScheduleCreate.Commands;

namespace ScheduleCreate.ViewModels
{
    public class StatisticsViewModel : ViewModelBase
    {
        private readonly IStatisticsService _statisticsService;
        private DateTime _startDate;
        private DateTime _endDate;
        private ObservableCollection<TeacherLoadStatistics> _teacherStatistics;
        private ObservableCollection<AuditoriumLoadStatistics> _auditoriumStatistics;
        private string _errorMessage;
        private Window _window;

        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TeacherLoadStatistics> TeacherStatistics
        {
            get => _teacherStatistics;
            set
            {
                _teacherStatistics = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<AuditoriumLoadStatistics> AuditoriumStatistics
        {
            get => _auditoriumStatistics;
            set
            {
                _auditoriumStatistics = value;
                OnPropertyChanged();
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand RefreshCommand { get; }
        public ICommand CloseCommand { get; }

        public StatisticsViewModel(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;

            _startDate = DateTime.Today;
            _endDate = DateTime.Today.AddDays(7);
            _teacherStatistics = new ObservableCollection<TeacherLoadStatistics>();
            _auditoriumStatistics = new ObservableCollection<AuditoriumLoadStatistics>();

            RefreshCommand = new RelayCommand(async _ => await LoadStatisticsAsync());
            CloseCommand = new RelayCommand(_ => Close());

            LoadStatisticsAsync().ConfigureAwait(false);
        }

        public void SetWindow(Window window)
        {
            _window = window;
        }

        private async Task LoadStatisticsAsync()
        {
            try
            {
                var teacherStats = await _statisticsService.GetTeacherLoadStatisticsAsync(StartDate, EndDate);
                TeacherStatistics = new ObservableCollection<TeacherLoadStatistics>(teacherStats);

                var auditoriumStats = await _statisticsService.GetAuditoriumLoadStatisticsAsync(StartDate, EndDate);
                AuditoriumStatistics = new ObservableCollection<AuditoriumLoadStatistics>(auditoriumStats);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при загрузке статистики: {ex.Message}";
            }
        }

        private void Close()
        {
            _window?.Close();
        }
    }
} 