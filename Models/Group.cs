using System;
using System.Collections.Generic;

namespace ScheduleCreate.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int StudentCount { get; set; }
        public int Course { get; set; }
        public string Faculty { get; set; } = string.Empty;
        public virtual ICollection<ScheduleEntry>? ScheduleEntries { get; set; }
    }
} 