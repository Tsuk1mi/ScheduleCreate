using System;
using System.Collections.Generic;

namespace ScheduleCreate.Models
{
    public class ScheduleEntry
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int TeacherId { get; set; }
        public Teacher? Teacher { get; set; }
        public int DisciplineId { get; set; }
        public Discipline? Discipline { get; set; }
        public List<Group> Groups { get; set; } = new List<Group>();
        public int AuditoriumId { get; set; }
        public Auditorium? Auditorium { get; set; }
        public ScheduleEntryType Type { get; set; }
        public bool IsStream { get; set; }
        public string? Notes { get; set; }
    }

    public enum ScheduleEntryType
    {
        Lecture,
        Practice,
        Laboratory,
        Exam
    }
} 