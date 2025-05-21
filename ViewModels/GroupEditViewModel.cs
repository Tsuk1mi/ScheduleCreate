using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using ScheduleCreate.Models;
using ScheduleCreate.Services;
using ScheduleCreate.Views;
using ScheduleCreate.Commands;

namespace ScheduleCreate.ViewModels
{
    public class GroupEditViewModel : ViewModelBase
    {
        private readonly IGroupService _groupService;
        private Group _group;
        private bool _isEditMode;
        private string _errorMessage = string.Empty;
        private Window? _window;

        public GroupEditViewModel(IGroupService groupService, Group? group = null)
        {
            _groupService = groupService;
            _isEditMode = group != null;
            _group = group ?? new Group();

            // Инициализация команд
            SaveCommand = new RelayCommand(async _ => await SaveAsync(), _ => CanSave());
            CancelCommand = new RelayCommand(_ => Cancel());
        }

        public void SetWindow(Window window)
        {
            _window = window;
        }

        public Group Group
        {
            get => _group;
            set => SetProperty(ref _group, value);
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
            if (string.IsNullOrWhiteSpace(Group.Name))
            {
                ErrorMessage = "Пожалуйста, введите название группы";
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
                    await _groupService.UpdateGroupAsync(Group);
                }
                else
                {
                    await _groupService.AddGroupAsync(Group);
                }

                if (_window != null)
                {
                    _window.DialogResult = true;
                    _window.Close();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при сохранении: {ex.Message}";
            }
        }

        private void Cancel()
        {
            if (_window != null)
            {
                _window.DialogResult = false;
                _window.Close();
            }
        }
    }
}
