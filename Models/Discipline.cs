using System;
using System.Collections.Generic;

namespace ScheduleCreate.Models
{
    public class Discipline
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public double TotalHours { get; set; }
        public double LectureHours { get; set; }
        public double PracticeHours { get; set; }
        public double LabHours { get; set; }
        public virtual ICollection<Teacher>? Teachers { get; set; }
        public virtual ICollection<ScheduleEntry>? ScheduleEntries { get; set; }
    }
} 