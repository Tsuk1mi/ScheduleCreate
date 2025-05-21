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
                .Where(e => e.Date >= startDate && e.Date <= endDate)
                .ToListAsync();
                
            var teachers = await _context.Teachers.ToListAsync();

            return teachers.Select(teacher => new TeacherLoadStatistics
            {
                TeacherName = teacher.Name,
                TotalHours = scheduleEntries.Count(e => e.TeacherId == teacher.Id) * 2 // Каждое занятие по 2 часа
            });
        }

        public async Task<IEnumerable<AuditoriumLoadStatistics>> GetAuditoriumLoadStatisticsAsync(DateTime startDate, DateTime endDate)
        {
            var scheduleEntries = await _context.ScheduleEntries
                .Where(e => e.Date >= startDate && e.Date <= endDate)
                .ToListAsync();
                
            var auditoriums = await _context.Auditoriums.ToListAsync();
            var totalPossibleHoursPerWeek = 60; // Примерное количество часов в неделю

            return auditoriums.Select(auditorium => new AuditoriumLoadStatistics
            {
                AuditoriumNumber = auditorium.Number,
                TotalHours = scheduleEntries.Count(e => e.AuditoriumId == auditorium.Id) * 2,
                UsagePercent = (scheduleEntries.Count(e => e.AuditoriumId == auditorium.Id) * 2.0 / totalPossibleHoursPerWeek) * 100
            });
        }
    }
}
