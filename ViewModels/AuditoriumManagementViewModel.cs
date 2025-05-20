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
    public class AuditoriumManagementViewModel : ViewModelBase
    {
        private readonly IAuditoriumService _auditoriumService;
        private ObservableCollection<Auditorium> _auditoriums;
        private Auditorium _selectedAuditorium;
        private string _errorMessage;
        private Window _window;

        public AuditoriumManagementViewModel(IAuditoriumService auditoriumService)
        {
            _auditoriumService = auditoriumService;
            _auditoriums = new ObservableCollection<Auditorium>();

            // Инициализация команд
            AddAuditoriumCommand = new RelayCommand(_ => AddAuditorium());
            EditAuditoriumCommand = new RelayCommand(_ => EditAuditorium(), _ => SelectedAuditorium != null);
            DeleteAuditoriumCommand = new RelayCommand(async _ => await DeleteAuditoriumAsync(), _ => SelectedAuditorium != null);
            CloseCommand = new RelayCommand(_ => Close());

            // Загрузка данных
            LoadDataAsync().ConfigureAwait(false);
        }

        public void SetWindow(Window window)
        {
            _window = window;
        }

        public ObservableCollection<Auditorium> Auditoriums
        {
            get => _auditoriums;
            set => SetProperty(ref _auditoriums, value);
        }

        public Auditorium SelectedAuditorium
        {
            get => _selectedAuditorium;
            set => SetProperty(ref _selectedAuditorium, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand AddAuditoriumCommand { get; }
        public ICommand EditAuditoriumCommand { get; }
        public ICommand DeleteAuditoriumCommand { get; }
        public ICommand CloseCommand { get; }

        private async Task LoadDataAsync()
        {
            try
            {
                var auditoriums = await _auditoriumService.GetAllAuditoriumsAsync();
                Auditoriums.Clear();
                foreach (var auditorium in auditoriums)
                {
                    Auditoriums.Add(auditorium);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при загрузке данных: {ex.Message}";
            }
        }

        private void AddAuditorium()
        {
            var viewModel = new AuditoriumEditViewModel(_auditoriumService);
            var window = new AuditoriumEditWindow(viewModel)
            {
                Owner = _window
            };

            if (window.ShowDialog() == true)
            {
                LoadDataAsync().ConfigureAwait(false);
            }
        }

        private void EditAuditorium()
        {
            if (SelectedAuditorium != null)
            {
                var viewModel = new AuditoriumEditViewModel(_auditoriumService, SelectedAuditorium);
                var window = new AuditoriumEditWindow(viewModel)
                {
                    Owner = _window
                };

                if (window.ShowDialog() == true)
                {
                    LoadDataAsync().ConfigureAwait(false);
                }
            }
        }

        private async Task DeleteAuditoriumAsync()
        {
            if (SelectedAuditorium != null)
            {
                try
                {
                    await _auditoriumService.DeleteAuditoriumAsync(SelectedAuditorium.Id);
                    Auditoriums.Remove(SelectedAuditorium);
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Ошибка при удалении аудитории: {ex.Message}";
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