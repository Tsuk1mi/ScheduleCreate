using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using ScheduleCreate.Models;
using ScheduleCreate.Services;
using ScheduleCreate.Views;
using System.IO;
using System.Linq;
using System.Windows;
using ScheduleCreate.Commands;
using Microsoft.Win32;
using System.Windows.Threading;

namespace ScheduleCreate.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IScheduleService _scheduleService;
        private readonly ITeacherService _teacherService;
        private readonly IGroupService _groupService;
        private readonly IAuditoriumService _auditoriumService;
        private readonly IScheduleExportService _scheduleExportService;
        private readonly IStatisticsService _statisticsService;
        private ObservableCollection<ScheduleEntry> _scheduleEntries;
        private ObservableCollection<ScheduleEntry> _filteredScheduleEntries;
        private ScheduleEntry _selectedScheduleEntry;
        private string _searchText;
        private DateTime? _startDate;
        private DateTime? _endDate;
        private Teacher? _selectedTeacher;
        private Group? _selectedGroup;
        private Auditorium? _selectedAuditorium;
        private string _errorMessage = string.Empty;
        private ObservableCollection<Teacher> _teachers = new ObservableCollection<Teacher>();
        private ObservableCollection<Group> _groups = new ObservableCollection<Group>();
        private ObservableCollection<Auditorium> _auditoriums = new ObservableCollection<Auditorium>();

        public ObservableCollection<ScheduleEntry> ScheduleEntries
        {
            get => _scheduleEntries;
            set
            {
                _scheduleEntries = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public ObservableCollection<ScheduleEntry> FilteredScheduleEntries
        {
            get => _filteredScheduleEntries;
            set
            {
                _filteredScheduleEntries = value;
                OnPropertyChanged();
            }
        }

        public ScheduleEntry SelectedScheduleEntry
        {
            get => _selectedScheduleEntry;
            set
            {
                _selectedScheduleEntry = value;
                OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public DateTime? StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public DateTime? EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public Teacher? SelectedTeacher
        {
            get => _selectedTeacher;
            set
            {
                _selectedTeacher = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public Group? SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                _selectedGroup = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public Auditorium? SelectedAuditorium
        {
            get => _selectedAuditorium;
            set
            {
                _selectedAuditorium = value;
                OnPropertyChanged();
                ApplyFilters();
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

        public ObservableCollection<Teacher> Teachers
        {
            get => _teachers;
            set => SetProperty(ref _teachers, value);
        }

        public ObservableCollection<Group> Groups
        {
            get => _groups;
            set => SetProperty(ref _groups, value);
        }

        public ObservableCollection<Auditorium> Auditoriums
        {
            get => _auditoriums;
            set => SetProperty(ref _auditoriums, value);
        }

        public ICommand AddScheduleEntryCommand { get; }
        public ICommand EditScheduleEntryCommand { get; }
        public ICommand DeleteScheduleEntryCommand { get; }
        public ICommand ManageTeachersCommand { get; }
        public ICommand ManageGroupsCommand { get; } 
        public ICommand ManageAuditoriumsCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand ImportCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand ShowStatisticsCommand { get; }

        public MainViewModel(
            IScheduleService scheduleService,
            ITeacherService teacherService,
            IGroupService groupService,
            IAuditoriumService auditoriumService,
            IScheduleExportService scheduleExportService,
            IStatisticsService statisticsService)
        {
            _scheduleService = scheduleService;
            _teacherService = teacherService;
            _groupService = groupService;
            _auditoriumService = auditoriumService;
            _scheduleExportService = scheduleExportService;
            _statisticsService = statisticsService;

            _scheduleEntries = new ObservableCollection<ScheduleEntry>();
            _filteredScheduleEntries = new ObservableCollection<ScheduleEntry>();

            AddScheduleEntryCommand = new RelayCommand(AddScheduleEntry);
            EditScheduleEntryCommand = new RelayCommand(EditScheduleEntry, CanEditOrDelete);
            DeleteScheduleEntryCommand = new RelayCommand(async _ => await DeleteScheduleEntryAsync(), CanEditOrDelete);
            ManageTeachersCommand = new RelayCommand(ManageTeachers);
            ManageGroupsCommand = new RelayCommand(ManageGroups); 
            ManageAuditoriumsCommand = new RelayCommand(ManageAuditoriums);
            ExportCommand = new RelayCommand(async _ => await Export());
            ImportCommand = new RelayCommand(async _ => await Import());
            ClearSearchCommand = new RelayCommand(ClearSearch);
            ShowStatisticsCommand = new RelayCommand(ShowStatistics);

            // Загрузка данных при инициализации
            // Важно дождаться завершения асинхронной операции перед продолжением
            LoadDataAsync().ConfigureAwait(false);
        }

        private async Task LoadDataAsync()
        {
            try
            {
                ScheduleEntries = new ObservableCollection<ScheduleEntry>(await _scheduleService.GetAllScheduleEntriesAsync());
                FilteredScheduleEntries = new ObservableCollection<ScheduleEntry>(ScheduleEntries);
                Teachers = new ObservableCollection<Teacher>(await _teacherService.GetAllTeachersAsync());
                Groups = new ObservableCollection<Group>(await _groupService.GetAllGroupsAsync());
                Auditoriums = new ObservableCollection<Auditorium>(await _auditoriumService.GetAllAuditoriumsAsync());
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при загрузке данных: {ex.Message}";
            }
        }

        private void ApplyFilters()
        {
            var filtered = ScheduleEntries.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(e =>
                    (e.Discipline?.Name?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (e.Teacher?.FullName?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (e.Auditorium?.Number?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    e.Groups.Any(g => g.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
            }

            if (StartDate.HasValue)
            {
                filtered = filtered.Where(e => e.Date.Date >= StartDate.Value.Date);
            }

            if (EndDate.HasValue)
            {
                filtered = filtered.Where(e => e.Date.Date <= EndDate.Value.Date);
            }

            if (SelectedTeacher != null)
            {
                filtered = filtered.Where(e => e.Teacher?.Id == SelectedTeacher.Id);
            }

            if (SelectedGroup != null)
            {
                filtered = filtered.Where(e => e.Groups.Any(g => g.Id == SelectedGroup.Id));
            }

            if (SelectedAuditorium != null)
            {
                filtered = filtered.Where(e => e.Auditorium?.Id == SelectedAuditorium.Id);
            }

            FilteredScheduleEntries = new ObservableCollection<ScheduleEntry>(filtered);
        }

        private void ClearSearch()
        {
            SearchText = string.Empty;
            StartDate = null;
            EndDate = null;
            SelectedTeacher = null;
            SelectedGroup = null;
            SelectedAuditorium = null;
            ApplyFilters();
        }

        private void AddScheduleEntry(object parameter)
        {
            var viewModel = new ScheduleEntryViewModel(_scheduleService, _teacherService, _groupService, _auditoriumService);
            var window = new ScheduleEntryWindow(viewModel)
            {
                Owner = App.Current.MainWindow
            };

            window.ShowDialog();
        }

        private void EditScheduleEntry(object parameter)
        {
            if (SelectedScheduleEntry != null)
            {
                var viewModel = new ScheduleEntryViewModel(_scheduleService, _teacherService, _groupService, _auditoriumService, SelectedScheduleEntry);
                var window = new ScheduleEntryWindow(viewModel)
                {
                    Owner = App.Current.MainWindow
                };

                window.ShowDialog();
            }
        }

        private async Task DeleteScheduleEntryAsync()
        {
            if (SelectedScheduleEntry != null)
            {
                try
                {
                    await _scheduleService.DeleteScheduleEntryAsync(SelectedScheduleEntry.Id);
                    ScheduleEntries.Remove(SelectedScheduleEntry);
                    ApplyFilters();
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Ошибка при удалении записи расписания: {ex.Message}";
                }
            }
        }

        private void ManageTeachers()
        {
            try
            {
                var viewModel = new TeacherManagementViewModel(_teacherService);
                var window = new TeacherManagementWindow(viewModel)
                {
                    Owner = Application.Current.MainWindow
                };

                window.ShowDialog();
                LoadDataAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при открытии окна управления преподавателями: {ex.Message}";
            }
        }

        private void ManageGroups()
        {
            try
            {
                var viewModel = new GroupManagementViewModel(_groupService);
                var window = new GroupManagementWindow(viewModel)
                {
                    Owner = Application.Current.MainWindow
                };

                window.ShowDialog();
                LoadDataAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при открытии окна управления группами: {ex.Message}";
            }
        }

        private void ManageAuditoriums()
        {
            try
            {
                var viewModel = new AuditoriumManagementViewModel(_auditoriumService);
                var window = new AuditoriumManagementWindow(viewModel)
                {
                    Owner = Application.Current.MainWindow
                };
                
                window.ShowDialog();
                LoadDataAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при открытии окна управления аудиториями: {ex.Message}";
            }
        }

        private async Task Export()
        {
            try
            {
                var dialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Excel файлы (*.xlsx)|*.xlsx|PDF файлы (*.pdf)|*.pdf",
                    DefaultExt = "xlsx"
                };

                if (dialog.ShowDialog() == true)
                {
                    var filePath = dialog.FileName;
                    var extension = Path.GetExtension(filePath).ToLower();

                    if (extension == ".xlsx")
                    {
                        await _scheduleExportService.ExportToExcelAsync(ScheduleEntries, filePath);
                        MessageBox.Show("Расписание успешно экспортировано в Excel.", "Экспорт", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else if (extension == ".pdf")
                    {
                        await _scheduleExportService.ExportToPdfAsync(ScheduleEntries, filePath);
                        MessageBox.Show("Расписание успешно экспортировано в PDF.", "Экспорт", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при экспорте расписания: {ex.Message}";
                MessageBox.Show($"Ошибка при экспорте: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task Import()
        {
            try
            {
                var dialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "Excel файлы (*.xlsx)|*.xlsx",
                    DefaultExt = "xlsx"
                };

                if (dialog.ShowDialog() == true)
                {
                    var entries = await _scheduleExportService.ImportFromExcelAsync(dialog.FileName);
                    
                    // Добавляем импортированные записи
                    foreach (var entry in entries)
                    {
                        await _scheduleService.AddScheduleEntryAsync(entry);
                    }

                    // Обновляем список
                    await LoadDataAsync();
                    MessageBox.Show("Расписание успешно импортировано из Excel.", "Импорт", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при импорте расписания: {ex.Message}";
                MessageBox.Show($"Ошибка при импорте: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowStatistics()
        {
            try
            {
                var viewModel = new StatisticsViewModel(_statisticsService);
                var window = new StatisticsWindow(viewModel)
                {
                    Owner = Application.Current.MainWindow
                };
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при открытии окна статистики: {ex.Message}";
            }
        }
    }
} 