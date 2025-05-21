using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ScheduleCreate.Data;
using ScheduleCreate.Models;

namespace ScheduleCreate.Services
{
    public class AuditoriumService : IAuditoriumService
    {
        private readonly AppDbContext _context;

        public AuditoriumService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Auditorium>> GetAllAuditoriumsAsync()
        {
            return await _context.Auditoriums.ToListAsync() ?? new List<Auditorium>();
        }

        public async Task<Auditorium> GetAuditoriumByIdAsync(int id)
        {
            var auditorium = await _context.Auditoriums.FindAsync(id);
            if (auditorium == null)
                throw new KeyNotFoundException($"Аудитория с ID {id} не найдена");
            return auditorium;
        }

        public async Task<Auditorium> AddAuditoriumAsync(Auditorium auditorium)
        {
            _context.Auditoriums.Add(auditorium);
            await _context.SaveChangesAsync();
            return auditorium;
        }

        public async Task<Auditorium> UpdateAuditoriumAsync(Auditorium auditorium)
        {
            var existing = await _context.Auditoriums.FindAsync(auditorium.Id)
                ?? throw new KeyNotFoundException($"Аудитория с ID {auditorium.Id} не найдена");
                
            _context.Entry(existing).CurrentValues.SetValues(auditorium);
            await _context.SaveChangesAsync();
            return auditorium;
        }

        public async Task DeleteAuditoriumAsync(int id)
        {
            var auditorium = await _context.Auditoriums.FindAsync(id)
                ?? throw new KeyNotFoundException($"Аудитория с ID {id} не найдена");
                
            _context.Auditoriums.Remove(auditorium);
            await _context.SaveChangesAsync();
        }
    }
}

