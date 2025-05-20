using System.Collections.Generic;
using System.Threading.Tasks;
using ScheduleCreate.Models;

namespace ScheduleCreate.Services
{
    public interface ITeacherService
    {
        Task<IEnumerable<Teacher>> GetAllTeachersAsync();
        Task<Teacher> GetTeacherByIdAsync(int id);
        Task<Teacher> AddTeacherAsync(Teacher teacher);
        Task<Teacher> UpdateTeacherAsync(Teacher teacher);
        Task DeleteTeacherAsync(int id);
        Task<IEnumerable<Discipline>> GetTeacherDisciplinesAsync(int teacherId);
        Task AddDisciplineToTeacherAsync(int teacherId, int disciplineId);
        Task RemoveDisciplineFromTeacherAsync(int teacherId, int disciplineId);
    }
} 