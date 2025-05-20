using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScheduleCreate.Models;

namespace ScheduleCreate.Services
{
    public interface IScheduleService
    {
        Task<IEnumerable<ScheduleEntry>> GetScheduleAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<ScheduleEntry>> GetScheduleByGroupAsync(int groupId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<ScheduleEntry>> GetScheduleByTeacherAsync(int teacherId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<ScheduleEntry>> GetScheduleByAuditoriumAsync(int auditoriumId, DateTime startDate, DateTime endDate);
        Task<ScheduleEntry> AddScheduleEntryAsync(ScheduleEntry entry);
        Task<ScheduleEntry> UpdateScheduleEntryAsync(ScheduleEntry entry);
        Task DeleteScheduleEntryAsync(int id);
        Task<bool> IsTimeSlotAvailableAsync(int auditoriumId, int teacherId, DateTime date, TimeSpan startTime, TimeSpan endTime);
        Task<double> CalculateTeacherLoadAsync(int teacherId, DateTime startDate, DateTime endDate);
    }
} 