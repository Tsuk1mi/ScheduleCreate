using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ScheduleCreate.Data;
using ScheduleCreate.Models;

namespace ScheduleCreate.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly AppDbContext _context;

        public ScheduleService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ScheduleEntry>> GetScheduleAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.ScheduleEntries
                .Include(s => s.Discipline)
                .Include(s => s.Teacher)
                .Include(s => s.Auditorium)
                .Include(s => s.Groups)
                .Where(s => s.Date >= startDate && s.Date <= endDate)
                .OrderBy(s => s.Date)
                .ThenBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<ScheduleEntry>> GetScheduleByGroupAsync(int groupId, DateTime startDate, DateTime endDate)
        {
            return await _context.ScheduleEntries
                .Include(s => s.Discipline)
                .Include(s => s.Teacher)
                .Include(s => s.Auditorium)
                .Include(s => s.Groups)
                .Where(s => s.Date >= startDate && s.Date <= endDate)
                .Where(s => s.Groups.Any(g => g.Id == groupId))
                .OrderBy(s => s.Date)
                .ThenBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<ScheduleEntry>> GetScheduleByTeacherAsync(int teacherId, DateTime startDate, DateTime endDate)
        {
            return await _context.ScheduleEntries
                .Include(s => s.Discipline)
                .Include(s => s.Teacher)
                .Include(s => s.Auditorium)
                .Include(s => s.Groups)
                .Where(s => s.Date >= startDate && s.Date <= endDate)
                .Where(s => s.TeacherId == teacherId)
                .OrderBy(s => s.Date)
                .ThenBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<ScheduleEntry>> GetScheduleByAuditoriumAsync(int auditoriumId, DateTime startDate, DateTime endDate)
        {
            return await _context.ScheduleEntries
                .Include(s => s.Discipline)
                .Include(s => s.Teacher)
                .Include(s => s.Auditorium)
                .Include(s => s.Groups)
                .Where(s => s.Date >= startDate && s.Date <= endDate)
                .Where(s => s.AuditoriumId == auditoriumId)
                .OrderBy(s => s.Date)
                .ThenBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<ScheduleEntry> AddScheduleEntryAsync(ScheduleEntry entry)
        {
            if (!await IsTimeSlotAvailableAsync(entry.AuditoriumId, entry.TeacherId, entry.Date, entry.StartTime, entry.EndTime))
            {
                throw new InvalidOperationException("Выбранное время занято");
            }

            _context.ScheduleEntries.Add(entry);
            await _context.SaveChangesAsync();
            return entry;
        }

        public async Task<ScheduleEntry> UpdateScheduleEntryAsync(ScheduleEntry entry)
        {
            var existingEntry = await _context.ScheduleEntries.FindAsync(entry.Id);
            if (existingEntry == null)
            {
                throw new KeyNotFoundException("Запись расписания не найдена");
            }

            if (!await IsTimeSlotAvailableAsync(entry.AuditoriumId, entry.TeacherId, entry.Date, entry.StartTime, entry.EndTime))
            {
                throw new InvalidOperationException("Выбранное время занято");
            }

            _context.Entry(existingEntry).CurrentValues.SetValues(entry);
            await _context.SaveChangesAsync();
            return entry;
        }

        public async Task DeleteScheduleEntryAsync(int id)
        {
            var entry = await _context.ScheduleEntries.FindAsync(id);
            if (entry == null)
            {
                throw new KeyNotFoundException("Запись расписания не найдена");
            }

            _context.ScheduleEntries.Remove(entry);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsTimeSlotAvailableAsync(int auditoriumId, int teacherId, DateTime date, TimeSpan startTime, TimeSpan endTime)
        {
            return !await _context.ScheduleEntries
                .AnyAsync(s => s.Date == date &&
                    ((s.StartTime <= startTime && s.EndTime > startTime) ||
                     (s.StartTime < endTime && s.EndTime >= endTime) ||
                     (s.StartTime >= startTime && s.EndTime <= endTime)) &&
                    (s.AuditoriumId == auditoriumId || s.TeacherId == teacherId));
        }

        public async Task<double> CalculateTeacherLoadAsync(int teacherId, DateTime startDate, DateTime endDate)
        {
            var entries = await _context.ScheduleEntries
                .Where(s => s.TeacherId == teacherId && s.Date >= startDate && s.Date <= endDate)
                .ToListAsync();

            return entries.Sum(e => (e.EndTime - e.StartTime).TotalHours);
        }
    }
} 