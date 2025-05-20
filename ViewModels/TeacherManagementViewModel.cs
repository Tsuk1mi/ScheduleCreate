using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using ScheduleCreate.Models;
using ScheduleCreate.Services;
using ScheduleCreate.Views;
using ScheduleCreate.Commands;

namespace ScheduleCreate.ViewModels
{
    public class TeacherManagementViewModel : ViewModelBase
    {
        private readonly ITeacherService _teacherService;
        private ObservableCollection<Teacher> _teachers;
        private Teacher? _selectedTeacher = null;
        private string _errorMessage = string.Empty;
        private Window? _window;

        public TeacherManagementViewModel(ITeacherService teacherService)
        {
            _teacherService = teacherService;
            _teachers = new ObservableCollection<Teacher>();

            // Инициализация команд
            AddTeacherCommand = new RelayCommand(_ => AddTeacher());
            EditTeacherCommand = new RelayCommand(_ => EditTeacher(), _ => SelectedTeacher != null);
            DeleteTeacherCommand = new RelayCommand(async _ => await DeleteTeacherAsync(), _ => SelectedTeacher != null);
            CloseCommand = new RelayCommand(_ => Close());

            // Загрузка данных
            LoadDataAsync().ConfigureAwait(false);
        }

        public void SetWindow(Window window)
        {
            _window = window;
        }

        public ObservableCollection<Teacher> Teachers
        {
            get => _teachers;
            set => SetProperty(ref _teachers, value);
        }

        public Teacher SelectedTeacher
        {
            get => _selectedTeacher;
            set => SetProperty(ref _selectedTeacher, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand AddTeacherCommand { get; }
        public ICommand EditTeacherCommand { get; }
        public ICommand DeleteTeacherCommand { get; }
        public ICommand CloseCommand { get; }

        private async Task LoadDataAsync()
        {
            try
            {
                var teachers = await _teacherService.GetAllTeachersAsync();
                Teachers.Clear();
                foreach (var teacher in teachers)
                {
                    Teachers.Add(teacher);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при загрузке данных: {ex.Message}";
            }
        }

        private void AddTeacher()
        {
            var viewModel = new TeacherEditViewModel(_teacherService);
            var window = new TeacherEditWindow(viewModel)
            {
                Owner = _window
            };

            if (window.ShowDialog() == true)
            {
                LoadDataAsync().ConfigureAwait(false);
            }
        }

        private void EditTeacher()
        {
            if (SelectedTeacher != null)
            {
                var viewModel = new TeacherEditViewModel(_teacherService, SelectedTeacher);
                var window = new TeacherEditWindow(viewModel)
                {
                    Owner = _window
                };

                if (window.ShowDialog() == true)
                {
                    LoadDataAsync().ConfigureAwait(false);
                }
            }
        }

        private async Task DeleteTeacherAsync()
        {
            if (SelectedTeacher != null)
            {
                try
                {
                    await _teacherService.DeleteTeacherAsync(SelectedTeacher.Id);
                    Teachers.Remove(SelectedTeacher);
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Ошибка при удалении преподавателя: {ex.Message}";
                }
            }
        }

        private void Close()
        {
            _window.DialogResult = true;
            _window.Close();
        }
    }
} 