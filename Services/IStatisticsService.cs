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
}
