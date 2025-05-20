using System.Collections.Generic;
using System.Threading.Tasks;
using ScheduleCreate.Models;

namespace ScheduleCreate.Services
{
    public interface IScheduleExportService
    {
        Task ExportToExcelAsync(IEnumerable<ScheduleEntry> entries, string filePath);
        Task<IEnumerable<ScheduleEntry>> ImportFromExcelAsync(string filePath);
        Task ExportToPdfAsync(IEnumerable<ScheduleEntry> entries, string filePath);
    }
} 