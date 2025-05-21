using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ScheduleCreate.Data;
using ScheduleCreate.Models;

namespace ScheduleCreate.Services
{
    public class DbInitializerService
    {
        private readonly AppDbContext _context;

        public DbInitializerService(AppDbContext context)
        {
            _context = context;
        }

        public async Task InitializeAsync()
        {
            // Проверяем, есть ли уже данные
            if (await _context.Teachers.AnyAsync())
                return;

            // Добавляем тестовых преподавателей
            var teachers = new[]
            {
                new Teacher { FullName = "Иванов И.И.", Department = "Кафедра информатики", LoadHours = 800 },
                new Teacher { FullName = "Петров П.П.", Department = "Кафедра математики", LoadHours = 600 },
                new Teacher { FullName = "Сидорова С.С.", Department = "Кафедра физики", LoadHours = 700 }
            };
            await _context.Teachers.AddRangeAsync(teachers);

            // Добавляем тестовые дисциплины
            var disciplines = new[]
            {
                new Discipline { Name = "Программирование", Code = "CS101", TotalHours = 144, LectureHours = 48, PracticeHours = 48, LabHours = 48 },
                new Discipline { Name = "Математический анализ", Code = "MA201", TotalHours = 180, LectureHours = 90, PracticeHours = 90 },
                new Discipline { Name = "Физика", Code = "PH101", TotalHours = 160, LectureHours = 64, PracticeHours = 48, LabHours = 48 }
            };
            await _context.Disciplines.AddRangeAsync(disciplines);

            // Добавляем тестовые группы
            var groups = new[]
            {
                new Group { Name = "ИВТ-101", StudentCount = 25, Course = 1 },
                new Group { Name = "ИВТ-201", StudentCount = 22, Course = 2 },
                new Group { Name = "ПИ-101", StudentCount = 20, Course = 1 }
            };
            await _context.Groups.AddRangeAsync(groups);

            // Добавляем тестовые аудитории
            var auditoriums = new[]
            {
                new Auditorium { Number = "101", Capacity = 30, Type = AuditoriumType.LectureHall },
                new Auditorium { Number = "102", Capacity = 25, Type = AuditoriumType.ComputerClass },
                new Auditorium { Number = "103", Capacity = 20, Type = AuditoriumType.Laboratory }
            };
            await _context.Auditoriums.AddRangeAsync(auditoriums);

            await _context.SaveChangesAsync();
        }
    }
}
