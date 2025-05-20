using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using ScheduleCreate.Models;
using ScheduleCreate.Services;
using ScheduleCreate.Views;
using ScheduleCreate.Commands;

namespace ScheduleCreate.ViewModels
{
    public class TeacherEditViewModel : ViewModelBase
    {
        private readonly ITeacherService _teacherService;
        private Teacher _teacher;
        private bool _isEditMode;
        private string _errorMessage = string.Empty;
        private Window? _window;

        public TeacherEditViewModel(ITeacherService teacherService, Teacher? teacher = null)
        {
            _teacherService = teacherService;
            _isEditMode = teacher != null;
            _teacher = teacher ?? new Teacher();

            // Инициализация команд
            SaveCommand = new RelayCommand(async _ => await SaveAsync(), _ => CanSave());
            CancelCommand = new RelayCommand(_ => Cancel());
        }

        public void SetWindow(Window window)
        {
            _window = window;
        }

        public Teacher Teacher
        {
            get => _teacher;
            set => SetProperty(ref _teacher, value);
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        private bool CanSave()
        {
            if (string.IsNullOrWhiteSpace(Teacher.FullName))
            {
                ErrorMessage = "Пожалуйста, введите ФИО преподавателя";
                return false;
            }

            ErrorMessage = string.Empty;
            return true;
        }

        private async Task SaveAsync()
        {
            try
            {
                if (IsEditMode)
                {
                    await _teacherService.UpdateTeacherAsync(Teacher);
                }
                else
                {
                    await _teacherService.AddTeacherAsync(Teacher);
                }

                _window.DialogResult = true;
                _window.Close();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при сохранении: {ex.Message}";
            }
        }

        private void Cancel()
        {
            _window.DialogResult = false;
            _window.Close();
        }
    }
} 