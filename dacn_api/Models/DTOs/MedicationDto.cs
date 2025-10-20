namespace dacn_api.Models.DTOs
{
    public class MedicationReminderDto
    {
        public int Id { get; set; }
        public string MedicineName { get; set; } = null!;
        public string? Dosage { get; set; }
        public string? Frequency { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public TimeOnly ReminderTime { get; set; }
        public string? Note { get; set; }
        public bool? IsActive { get; set; }
    }

    public class CreateMedicationReminderDto
    {
        public string MedicineName { get; set; } = null!;
        public string? Dosage { get; set; }
        public string? Frequency { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public TimeOnly ReminderTime { get; set; }
        public string? Note { get; set; }
        public bool? IsActive { get; set; } = true;
    }

}
