namespace dacn_api.Models.DTOs
{
    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public string? FullName { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public double? Height { get; set; }
        public double? LatestWeight { get; set; }
        public double? Bmi { get; set; }
    }

    public class IdealStatsDto
    {
        public double? IdealWeight { get; set; }
        public double? IdealWaterMl { get; set; }
        public double? IdealCaloriesIn { get; set; }
        public double? IdealCaloriesOut { get; set; }
    }
}
