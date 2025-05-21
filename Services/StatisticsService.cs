using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ScheduleCreate.Data;
using ScheduleCreate.Models;

namespace ScheduleCreate.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly AppDbContext _context;

        public StatisticsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TeacherLoadStatistics>> GetTeacherLoadStatisticsAsync(DateTime startDate, DateTime endDate)
        {
            var scheduleEntries = await _context.ScheduleEntries
                .Include(s => s.Teacher)
                .Where(s => s.Date >= startDate && s.Date <= endDate && s.Teacher != null)
                .ToListAsync();

            return scheduleEntries
                .GroupBy(s => s.Teacher)
                .Select(g => new TeacherLoadStatistics
                {
                    TeacherName = g.Key?.FullName ?? "Неизвестный преподаватель",
                    TotalHours = (int)Math.Round(g.Sum(s => (s.EndTime - s.StartTime).TotalHours))
                })
                .OrderByDescending(s => s.TotalHours);
        }

        public async Task<IEnumerable<AuditoriumLoadStatistics>> GetAuditoriumLoadStatisticsAsync(DateTime startDate, DateTime endDate)
        {
            var scheduleEntries = await _context.ScheduleEntries
                .Include(s => s.Auditorium)
                .Where(s => s.Date >= startDate && s.Date <= endDate && s.Auditorium != null)
                .ToListAsync();

            return scheduleEntries
                .GroupBy(s => s.Auditorium)
                .Select(g => new AuditoriumLoadStatistics
                {
                    AuditoriumNumber = g.Key?.Number ?? "Неизвестная аудитория",
                    TotalHours = (int)Math.Round(g.Sum(s => (s.EndTime - s.StartTime).TotalHours)),
                    UsagePercent = CalculateUsagePercent(g.Sum(s => (s.EndTime - s.StartTime).TotalHours), startDate, endDate)
                })
                .OrderByDescending(s => s.TotalHours);
        }

        private static double CalculateUsagePercent(double totalHours, DateTime startDate, DateTime endDate)
        {
            var workingDays = CountWorkingDays(startDate, endDate);
            var totalPossibleHours = workingDays * 12.0; // Предполагаем 12-часовой рабочий день
            return (totalHours / totalPossibleHours) * 100;
        }

        private static int CountWorkingDays(DateTime startDate, DateTime endDate)
        {
            var days = 0;
            var current = startDate.Date;
            while (current <= endDate.Date)
            {
                if (current.DayOfWeek != DayOfWeek.Saturday && current.DayOfWeek != DayOfWeek.Sunday)
                {
                    days++;
                }
                current = current.AddDays(1);
            }
            return days;
        }
    }
}

