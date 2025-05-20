using System.Collections.Generic;
using System.Threading.Tasks;
using ScheduleCreate.Models;

namespace ScheduleCreate.Services
{
    public interface IAuditoriumService
    {
        Task<IEnumerable<Auditorium>> GetAllAuditoriumsAsync();
        Task<Auditorium> GetAuditoriumByIdAsync(int id);
        Task<Auditorium> AddAuditoriumAsync(Auditorium auditorium);
        Task<Auditorium> UpdateAuditoriumAsync(Auditorium auditorium);
        Task DeleteAuditoriumAsync(int id);
    }
} 