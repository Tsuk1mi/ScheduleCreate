using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScheduleCreate.Models;

namespace ScheduleCreate.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IScheduleService _scheduleService;

        public StatisticsService(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        public async Task<IEnumerable<TeacherLoadStatistics>> GetTeacherLoadStatisticsAsync(DateTime startDate, DateTime endDate)
        {
            var entries = await _scheduleService.GetScheduleAsync(startDate, endDate);
            
            return entries
                .GroupBy(e => e.Teacher)
                .Select(g => new TeacherLoadStatistics
                {
                    Teacher = g.Key,
                    TotalHours = g.Sum(e => (int)(e.EndTime - e.StartTime).TotalHours),
                    TotalGroups = g.SelectMany(e => e.Groups).Distinct().Count(),
                    DisciplineHours = g.GroupBy(e => e.Discipline.Name)
                        .ToDictionary(
                            d => d.Key,
                            d => d.Sum(e => (int)(e.EndTime - e.StartTime).TotalHours)
                        )
                });
        }

        public async Task<IEnumerable<AuditoriumLoadStatistics>> GetAuditoriumLoadStatisticsAsync(DateTime startDate, DateTime endDate)
        {
            var entries = await _scheduleService.GetScheduleAsync(startDate, endDate);
            
            return entries
                .GroupBy(e => e.Auditorium)
                .Select(g => new AuditoriumLoadStatistics
                {
                    Auditorium = g.Key,
                    TotalHours = g.Sum(e => (int)(e.EndTime - e.StartTime).TotalHours),
                    TotalGroups = g.SelectMany(e => e.Groups).Distinct().Count(),
                    DisciplineHours = g.GroupBy(e => e.Discipline.Name)
                        .ToDictionary(
                            d => d.Key,
                            d => d.Sum(e => (int)(e.EndTime - e.StartTime).TotalHours)
                        )
                });
        }
    }
} 