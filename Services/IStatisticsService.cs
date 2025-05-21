using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScheduleCreate.Models;

namespace ScheduleCreate.Services
{
    public interface IStatisticsService
    {
        Task<IEnumerable<TeacherLoadStatistics>> GetTeacherLoadStatisticsAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<AuditoriumLoadStatistics>> GetAuditoriumLoadStatisticsAsync(DateTime startDate, DateTime endDate);
    }

    public class TeacherLoadStatistics
    {
        public TeacherLoadStatistics()
        {
            Teacher = null!;
            DisciplineHours = new Dictionary<string, int>();
        }
        public Teacher Teacher { get; set; }
        public int TotalHours { get; set; }
        public int TotalGroups { get; set; }
        public Dictionary<string, int> DisciplineHours { get; set; }
    }

    public class AuditoriumLoadStatistics
    {
        public AuditoriumLoadStatistics()
        {
            DisciplineHours = new Dictionary<string, int>();
        }
        public Auditorium? Auditorium { get; set; }
        public int TotalHours { get; set; }
        public int TotalGroups { get; set; }
        public Dictionary<string, int> DisciplineHours { get; set; }
    }
}
