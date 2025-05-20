using System;
using System.Collections.Generic;
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
    public class ScheduleEntryViewModel : ViewModelBase
    {
        private readonly IScheduleService _scheduleService;
        private readonly ITeacherService _teacherService;
        private readonly IGroupService _groupService;
        private readonly IAuditoriumService _auditoriumService;
        private ScheduleEntry _scheduleEntry;
        private bool _isEditMode;
        private ObservableCollection<Teacher> _teachers;
        private ObservableCollection<Discipline> _disciplines;
        private ObservableCollection<Group> _groups;
        private ObservableCollection<Auditorium> _auditoriums;
        private ObservableCollection<Group> _selectedGroups;
        private Window? _window;
        private string _errorMessage = string.Empty;

        public ScheduleEntryViewModel(
            IScheduleService scheduleService,
            ITeacherService teacherService,
            IGroupService groupService,
            IAuditoriumService auditoriumService,
            ScheduleEntry? scheduleEntry = null)
        {
            _scheduleService = scheduleService;
            _teacherService = teacherService;
            _groupService = groupService;
            _auditoriumService = auditoriumService;
            _isEditMode = scheduleEntry != null;
            _scheduleEntry = scheduleEntry ?? new ScheduleEntry
            {
                Date = DateTime.Today,
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(10, 30, 0)
            };

            // Инициализация коллекций
            _teachers = new ObservableCollection<Teacher>();
            _disciplines = new ObservableCollection<Discipline>();
            _groups = new ObservableCollection<Group>();
            _auditoriums = new ObservableCollection<Auditorium>();
            _selectedGroups = new ObservableCollection<Group>();

            // Инициализация команд
            SaveCommand = new RelayCommand(async _ => await SaveAsync(), _ => CanSave());
            CancelCommand = new RelayCommand(_ => Cancel());
            AddGroupCommand = new RelayCommand(_ => AddGroup(), _ => SelectedGroup != null);
            RemoveGroupCommand = new RelayCommand(_ => RemoveGroup(), _ => SelectedGroupToRemove != null);

            // Загрузка данных
            LoadDataAsync().ConfigureAwait(false);
        }

        public void SetWindow(Window window)
        {
            _window = window;
        }

        public ScheduleEntry ScheduleEntry
        {
            get => _scheduleEntry;
            set => SetProperty(ref _scheduleEntry, value);
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

        public ObservableCollection<Teacher> Teachers
        {
            get => _teachers;
            set => SetProperty(ref _teachers, value);
        }

        public ObservableCollection<Discipline> Disciplines
        {
            get => _disciplines;
            set => SetProperty(ref _disciplines, value);
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

        public ObservableCollection<Group> SelectedGroups
        {
            get => _selectedGroups;
            set => SetProperty(ref _selectedGroups, value);
        }

        public Teacher? SelectedTeacher
        {
            get => ScheduleEntry.Teacher;
            set
            {
                if (ScheduleEntry.Teacher != value)
                {
                    ScheduleEntry.Teacher = value;
                    ScheduleEntry.TeacherId = value?.Id ?? 0;
                    OnPropertyChanged();
                    UpdateAvailableDisciplines();
                }
            }
        }

        public Discipline? SelectedDiscipline
        {
            get => ScheduleEntry.Discipline;
            set
            {
                if (ScheduleEntry.Discipline != value)
                {
                    ScheduleEntry.Discipline = value;
                    ScheduleEntry.DisciplineId = value?.Id ?? 0;
                    OnPropertyChanged();
                }
            }
        }

        public Group SelectedGroup { get; set; }
        public Group SelectedGroupToRemove { get; set; }

        public Auditorium? SelectedAuditorium
        {
            get => ScheduleEntry.Auditorium;
            set
            {
                if (ScheduleEntry.Auditorium != value)
                {
                    ScheduleEntry.Auditorium = value;
                    ScheduleEntry.AuditoriumId = value?.Id ?? 0;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand AddGroupCommand { get; }
        public ICommand RemoveGroupCommand { get; }

        private async Task LoadDataAsync()
        {
            try
            {
                // Загрузка преподавателей
                var teachers = await _teacherService.GetAllTeachersAsync();
                Teachers.Clear();
                foreach (var teacher in teachers)
                {
                    Teachers.Add(teacher);
                }

                // Загрузка групп
                var groups = await _groupService.GetAllGroupsAsync();
                Groups.Clear();
                foreach (var group in groups)
                {
                    Groups.Add(group);
                }

                // Загрузка аудиторий
                var auditoriums = await _auditoriumService.GetAllAuditoriumsAsync();
                Auditoriums.Clear();
                foreach (var auditorium in auditoriums)
                {
                    Auditoriums.Add(auditorium);
                }

                if (IsEditMode)
                {
                    // Загрузка связанных данных для редактирования
                    SelectedTeacher = Teachers.FirstOrDefault(t => t.Id == ScheduleEntry.TeacherId);
                    SelectedAuditorium = Auditoriums.FirstOrDefault(a => a.Id == ScheduleEntry.AuditoriumId);
                    
                    // Загрузка выбранных групп
                    SelectedGroups.Clear();
                    foreach (var group in ScheduleEntry.Groups)
                    {
                        SelectedGroups.Add(group);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при загрузке данных: {ex.Message}";
            }
        }

        private void UpdateAvailableDisciplines()
        {
            if (SelectedTeacher != null)
            {
                var teacherDisciplines = SelectedTeacher.Disciplines;
                Disciplines.Clear();
                foreach (var discipline in teacherDisciplines)
                {
                    Disciplines.Add(discipline);
                }
            }
            else
            {
                Disciplines.Clear();
            }
        }

        private void AddGroup()
        {
            if (SelectedGroup != null && !SelectedGroups.Contains(SelectedGroup))
            {
                SelectedGroups.Add(SelectedGroup);
                ScheduleEntry.Groups.Add(SelectedGroup);
            }
        }

        private void RemoveGroup()
        {
            if (SelectedGroupToRemove != null)
            {
                SelectedGroups.Remove(SelectedGroupToRemove);
                ScheduleEntry.Groups.Remove(SelectedGroupToRemove);
            }
        }

        private bool CanSave()
        {
            if (ScheduleEntry == null) return false;
            if (string.IsNullOrWhiteSpace(ScheduleEntry.Discipline?.Name)) return false;
            if (ScheduleEntry.Teacher == null) return false;
            if (ScheduleEntry.Groups == null || ScheduleEntry.Groups.Count == 0) return false;
            if (ScheduleEntry.Auditorium == null) return false;
            if (ScheduleEntry.StartTime >= ScheduleEntry.EndTime) return false;

            // Проверка на пересечение занятий
            var hasOverlap = CheckScheduleOverlap();
            if (hasOverlap)
            {
                return false;
            }

            return true;
        }

        private bool CheckScheduleOverlap()
        {
            try
            {
                // Получаем все занятия на выбранную дату
                var schedule = _scheduleService.GetScheduleAsync(ScheduleEntry.Date, ScheduleEntry.Date).Result;

                // Проверяем пересечения для каждой группы
                foreach (var group in ScheduleEntry.Groups)
                {
                    var groupOverlaps = schedule
                        .Where(e => e.Id != ScheduleEntry.Id && // Исключаем текущее занятие при редактировании
                                  e.Groups.Any(g => g.Id == group.Id) && // Проверяем группы
                                  IsTimeOverlap(e.StartTime, e.EndTime, ScheduleEntry.StartTime, ScheduleEntry.EndTime))
                        .ToList();

                    if (groupOverlaps.Any())
                    {
                        var overlap = groupOverlaps.First();
                        ErrorMessage = $"Группа {group.Name} уже имеет занятие в это время: {overlap.StartTime:hh\\:mm} - {overlap.EndTime:hh\\:mm}";
                        return true;
                    }
                }

                // Проверяем пересечения для преподавателя
                var teacherOverlaps = schedule
                    .Where(e => e.Id != ScheduleEntry.Id && // Исключаем текущее занятие при редактировании
                              e.TeacherId == ScheduleEntry.TeacherId && // Проверяем преподавателя
                              IsTimeOverlap(e.StartTime, e.EndTime, ScheduleEntry.StartTime, ScheduleEntry.EndTime))
                    .ToList();

                if (teacherOverlaps.Any())
                {
                    var overlap = teacherOverlaps.First();
                    ErrorMessage = $"Преподаватель {ScheduleEntry.Teacher.FullName} уже имеет занятие в это время: {overlap.StartTime:hh\\:mm} - {overlap.EndTime:hh\\:mm}";
                    return true;
                }

                // Проверяем пересечения для аудитории
                var auditoriumOverlaps = schedule
                    .Where(e => e.Id != ScheduleEntry.Id && // Исключаем текущее занятие при редактировании
                              e.AuditoriumId == ScheduleEntry.AuditoriumId && // Проверяем аудиторию
                              IsTimeOverlap(e.StartTime, e.EndTime, ScheduleEntry.StartTime, ScheduleEntry.EndTime))
                    .ToList();

                if (auditoriumOverlaps.Any())
                {
                    var overlap = auditoriumOverlaps.First();
                    ErrorMessage = $"Аудитория {ScheduleEntry.Auditorium.Number} уже занята в это время: {overlap.StartTime:hh\\:mm} - {overlap.EndTime:hh\\:mm}";
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при проверке пересечений: {ex.Message}";
                return true;
            }
        }

        private bool IsTimeOverlap(TimeSpan start1, TimeSpan end1, TimeSpan start2, TimeSpan end2)
        {
            return start1 < end2 && start2 < end1;
        }

        private async Task SaveAsync()
        {
            try
            {
                if (IsEditMode)
                {
                    await _scheduleService.UpdateScheduleEntryAsync(ScheduleEntry);
                }
                else
                {
                    await _scheduleService.AddScheduleEntryAsync(ScheduleEntry);
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