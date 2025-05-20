using System;
using System.Collections.Generic;

namespace ScheduleCreate.Models
{
    public class Auditorium
    {
        public int Id { get; set; }
        public string Number { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public AuditoriumType Type { get; set; }
        public virtual ICollection<ScheduleEntry>? ScheduleEntries { get; set; }
    }
} 