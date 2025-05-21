using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ScheduleCreate.Data;
using ScheduleCreate.Models;

namespace ScheduleCreate.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly AppDbContext _context;

        public TeacherService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Teacher>> GetAllTeachersAsync()
        {
            return await _context.Teachers
                .Include(t => t.Disciplines)
                .OrderBy(t => t.FullName)
                .ToListAsync() ?? new List<Teacher>();
        }

        public async Task<Teacher> GetTeacherByIdAsync(int id)
        {
            var teacher = await _context.Teachers
                .Include(t => t.Disciplines)
                .FirstOrDefaultAsync(t => t.Id == id);
            
            if (teacher == null)
                throw new KeyNotFoundException($"Преподаватель с ID {id} не найден");
                
            return teacher;
        }

        public async Task<Teacher> AddTeacherAsync(Teacher teacher)
        {
            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();
            return teacher;
        }

        public async Task<Teacher> UpdateTeacherAsync(Teacher teacher)
        {
            var existingTeacher = await _context.Teachers.FindAsync(teacher.Id) 
                ?? throw new KeyNotFoundException($"Преподаватель с ID {teacher.Id} не найден");
            
            _context.Entry(existingTeacher).CurrentValues.SetValues(teacher);
            await _context.SaveChangesAsync();
            return existingTeacher;
        }

        public async Task DeleteTeacherAsync(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id)
                ?? throw new KeyNotFoundException($"Преподаватель с ID {id} не найден");
            
            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Discipline>> GetTeacherDisciplinesAsync(int teacherId)
        {
            var teacher = await _context.Teachers
                .Include(t => t.Disciplines)
                .FirstOrDefaultAsync(t => t.Id == teacherId);

            return teacher?.Disciplines ?? new List<Discipline>();
        }

        public async Task AddDisciplineToTeacherAsync(int teacherId, int disciplineId)
        {
            var teacher = await _context.Teachers
                .Include(t => t.Disciplines)
                .FirstOrDefaultAsync(t => t.Id == teacherId);

            var discipline = await _context.Disciplines.FindAsync(disciplineId);

            if (teacher == null || discipline == null)
            {
                throw new KeyNotFoundException("Преподаватель или дисциплина не найдены");
            }

            if (!teacher.Disciplines.Contains(discipline))
            {
                teacher.Disciplines.Add(discipline);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveDisciplineFromTeacherAsync(int teacherId, int disciplineId)
        {
            var teacher = await _context.Teachers
                .Include(t => t.Disciplines)
                .FirstOrDefaultAsync(t => t.Id == teacherId);

            var discipline = await _context.Disciplines.FindAsync(disciplineId);

            if (teacher == null || discipline == null)
            {
                throw new KeyNotFoundException("Преподаватель или дисциплина не найдены");
            }

            if (teacher.Disciplines.Contains(discipline))
            {
                teacher.Disciplines.Remove(discipline);
                await _context.SaveChangesAsync();
            }
        }
    }
}

