namespace dacn_api.Models.DTOs
{
    public class WeightRecordDto
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public double Weight { get; set; }
        public double? Bmi { get; set; }
        public double? IdealWeight { get; set; }
        public string? Note { get; set; }
    }

    public class CreateWeightRecordDto
    {

        public DateOnly Date { get; set; }
        public double Weight { get; set; }
        public string? Note { get; set; }
    }

}
