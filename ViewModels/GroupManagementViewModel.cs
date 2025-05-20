using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using ScheduleCreate.Models;
using ScheduleCreate.Services;
using ScheduleCreate.Views;
using System.Windows;
using ScheduleCreate.Commands;

namespace ScheduleCreate.ViewModels
{
    public class GroupManagementViewModel : ViewModelBase
    {
        private readonly IGroupService _groupService;
        private ObservableCollection<Group> _groups;
        private Group _selectedGroup;
        private string _errorMessage;
        private Window _window;

        public GroupManagementViewModel(IGroupService groupService)
        {
            _groupService = groupService;
            _groups = new ObservableCollection<Group>();

            // Инициализация команд
            AddGroupCommand = new RelayCommand(_ => AddGroup());
            EditGroupCommand = new RelayCommand(_ => EditGroup(), _ => SelectedGroup != null);
            DeleteGroupCommand = new RelayCommand(async _ => await DeleteGroupAsync(), _ => SelectedGroup != null);
            CloseCommand = new RelayCommand(_ => Close());

            // Загрузка данных
            LoadDataAsync().ConfigureAwait(false);
        }

        public void SetWindow(Window window)
        {
            _window = window;
        }

        public ObservableCollection<Group> Groups
        {
            get => _groups;
            set => SetProperty(ref _groups, value);
        }

        public Group SelectedGroup
        {
            get => _selectedGroup;
            set => SetProperty(ref _selectedGroup, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand AddGroupCommand { get; }
        public ICommand EditGroupCommand { get; }
        public ICommand DeleteGroupCommand { get; }
        public ICommand CloseCommand { get; }

        private async Task LoadDataAsync()
        {
            try
            {
                var groups = await _groupService.GetAllGroupsAsync();
                Groups.Clear();
                foreach (var group in groups)
                {
                    Groups.Add(group);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при загрузке данных: {ex.Message}";
            }
        }

        private void AddGroup()
        {
            var viewModel = new GroupEditViewModel(_groupService);
            var window = new GroupEditWindow(viewModel)
            {
                Owner = _window
            };

            if (window.ShowDialog() == true)
            {
                LoadDataAsync().ConfigureAwait(false);
            }
        }

        private void EditGroup()
        {
            if (SelectedGroup != null)
            {
                var viewModel = new GroupEditViewModel(_groupService, SelectedGroup);
                var window = new GroupEditWindow(viewModel)
                {
                    Owner = _window
                };

                if (window.ShowDialog() == true)
                {
                    LoadDataAsync().ConfigureAwait(false);
                }
            }
        }

        private async Task DeleteGroupAsync()
        {
            if (SelectedGroup != null)
            {
                try
                {
                    await _groupService.DeleteGroupAsync(SelectedGroup.Id);
                    Groups.Remove(SelectedGroup);
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Ошибка при удалении группы: {ex.Message}";
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