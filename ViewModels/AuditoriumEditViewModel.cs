using ScheduleCreate.Models;
using ScheduleCreate.Services;
using System;
using System.Windows.Input;
using System.Windows;
using ScheduleCreate.Commands;

namespace ScheduleCreate.ViewModels
{
    public class AuditoriumEditViewModel : ViewModelBase
    {
        private Auditorium _auditorium;
        private IAuditoriumService _auditoriumService;
        private bool _isEditMode;
        private string _errorMessage = string.Empty;
        private Window? _window;

        public Auditorium Auditorium
        {
            get => _auditorium;
            set => SetProperty(ref _auditorium, value);
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

        // Конструктор для добавления новой аудитории
        public AuditoriumEditViewModel(IAuditoriumService auditoriumService)
        {
            _auditoriumService = auditoriumService;
            _auditorium = new Auditorium();
            _isEditMode = false;
            SaveCommand = new RelayCommand(async _ => await Save(), _ => CanSave());
            CancelCommand = new RelayCommand(_ => Cancel());
        }

        // Конструктор для редактирования существующей аудитории
        public AuditoriumEditViewModel(IAuditoriumService auditoriumService, Auditorium auditorium)
        {
            _auditoriumService = auditoriumService;
            _auditorium = auditorium;
            _isEditMode = true;
            SaveCommand = new RelayCommand(async _ => await Save(), _ => CanSave());
            CancelCommand = new RelayCommand(_ => Cancel());
        }

        public void SetWindow(Window window)
        {
            _window = window;
        }

        private async System.Threading.Tasks.Task Save()
        {
            try
            {
                if (_isEditMode)
                {
                    await _auditoriumService.UpdateAuditoriumAsync(_auditorium);
                }
                else
                {
                    await _auditoriumService.AddAuditoriumAsync(_auditorium);
                }
                _window.DialogResult = true;
                _window.Close();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при сохранении: {ex.Message}";
            }
        }

        private bool CanSave()
        {
            // Простая валидация: номер аудитории не должен быть пустым
            return !string.IsNullOrEmpty(_auditorium?.Number);
        }

        private void Cancel()
        {
            _window.DialogResult = false;
            _window.Close();
        }
    }
} 