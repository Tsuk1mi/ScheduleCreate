using System;
using System.Collections.Generic;

namespace ScheduleCreate.Models
{
    public class Teacher
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Department { get; set; }
        public double LoadHours { get; set; }
        public string? ContactInfo { get; set; }
        public virtual ICollection<Discipline>? Disciplines { get; set; }
    }
} 