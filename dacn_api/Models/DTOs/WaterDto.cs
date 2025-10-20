namespace dacn_api.Models.DTOs
{
    public class WaterRecordDto
    {
        public int Id { get; set; }
        public DateTime? Time { get; set; }
        public DateOnly Date { get; set; }
        public int Amount { get; set; }
        public int? Target { get; set; }
    }

    public class CreateWaterRecordDto
    {
        public DateOnly Date { get; set; }
        public int Amount { get; set; }
        public int? Target { get; set; }
        public DateTime? Time { get; set; }
    }

}
