namespace dacn_api.Models.DTOs
{
    public class SleepRecordDto
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int? DurationMinutes { get; set; }
        public string? SleepQuality { get; set; }
        public string? SleepType { get; set; }
        public string? Notes { get; set; }
    }

    public class CreateSleepRecordDto
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? SleepQuality { get; set; }
        public string? SleepType { get; set; }
        public string? Notes { get; set; }
    }

}
