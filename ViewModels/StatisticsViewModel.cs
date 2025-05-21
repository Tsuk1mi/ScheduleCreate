using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ScheduleCreate.Models;  // Явное указание на пространство имен моделей
using ScheduleCreate.Services;
using ScheduleCreate.Commands;

namespace ScheduleCreate.ViewModels
{
    public class StatisticsViewModel : ViewModelBase
    {
        private readonly IStatisticsService _statisticsService;
        private DateTime _startDate = DateTime.Today;
        private DateTime _endDate = DateTime.Today.AddDays(7);
        private ObservableCollection<Models.TeacherLoadStatistics> _teacherStatistics = new();
        private ObservableCollection<Models.AuditoriumLoadStatistics> _auditoriumStatistics = new();
        private string _errorMessage = string.Empty;
        private Window? _window;

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

        public ObservableCollection<Models.TeacherLoadStatistics> TeacherStatistics
        {
            get => _teacherStatistics;
            set
            {
                _teacherStatistics = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Models.AuditoriumLoadStatistics> AuditoriumStatistics
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
            _statisticsService = statisticsService ?? throw new ArgumentNullException(nameof(statisticsService));

            RefreshCommand = new RelayCommand(async _ => await LoadStatisticsAsync());
            CloseCommand = new RelayCommand(_ => Close());

            // Загружаем статистику при создании
            LoadStatisticsAsync().ConfigureAwait(false);
        }

        public void SetWindow(Window window)
        {
            _window = window ?? throw new ArgumentNullException(nameof(window));
        }

        private async Task LoadStatisticsAsync()
        {
            try
            {
                ErrorMessage = string.Empty;
                var teacherStats = await _statisticsService.GetTeacherLoadStatisticsAsync(StartDate, EndDate);
                TeacherStatistics = new ObservableCollection<Models.TeacherLoadStatistics>(teacherStats);

                var auditoriumStats = await _statisticsService.GetAuditoriumLoadStatisticsAsync(StartDate, EndDate);
                AuditoriumStatistics = new ObservableCollection<Models.AuditoriumLoadStatistics>(auditoriumStats);
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

