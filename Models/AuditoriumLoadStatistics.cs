namespace ScheduleCreate.Models
{
    public class AuditoriumLoadStatistics
    {
        public string AuditoriumNumber { get; set; } = string.Empty;
        public int TotalHours { get; set; }
        public double UsagePercent { get; set; }
    }
}

