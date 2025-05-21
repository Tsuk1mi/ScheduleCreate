using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ScheduleCreate.Data;
using ScheduleCreate.Models;

namespace ScheduleCreate.Services
{
    public class GroupService : IGroupService
    {
        private readonly AppDbContext _context;

        public GroupService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Group>> GetAllGroupsAsync()
        {
            return await _context.Groups.ToListAsync() ?? new List<Group>();
        }

        public async Task<Group> GetGroupByIdAsync(int id)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null)
                throw new KeyNotFoundException($"Группа с ID {id} не найдена");
            return group;
        }

        public async Task<Group> AddGroupAsync(Group group)
        {
            _context.Groups.Add(group);
            await _context.SaveChangesAsync();
            return group;
        }

        public async Task<Group> UpdateGroupAsync(Group group)
        {
            var existingGroup = await _context.Groups.FindAsync(group.Id) 
                ?? throw new KeyNotFoundException($"Группа с ID {group.Id} не найдена");
                
            _context.Entry(existingGroup).CurrentValues.SetValues(group);
            await _context.SaveChangesAsync();
            return existingGroup;
        }

        public async Task DeleteGroupAsync(int id)
        {
            var group = await _context.Groups.FindAsync(id) 
                ?? throw new KeyNotFoundException($"Группа с ID {id} не найдена");
                
            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();
        }
    }
}

