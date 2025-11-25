using System.ComponentModel.DataAnnotations;

namespace dacn_api.Models.DTOs
{
    public class ActivityRecordDto
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public string ActivityType { get; set; } = null!;
        public int? Duration { get; set; } // Đơn vị: Phút
        public double? CaloriesOut { get; set; }
        public int? Steps { get; set; }
        public double? Distance { get; set; } // Đơn vị: Km
    }
    public class CreateActivityRecordDto
    {
        [Required(ErrorMessage = "Ngày hoạt động là bắt buộc.")]
        public DateOnly Date { get; set; }

        [Required(ErrorMessage = "Loại hoạt động là bắt buộc.")]
        [StringLength(50, ErrorMessage = "Loại hoạt động không được vượt quá 50 ký tự.")]
        public string ActivityType { get; set; } = null!;

        // Các trường tùy chọn
        public int? Duration { get; set; } // Phút
        public double? CaloriesOut { get; set; }
        public int? Steps { get; set; }
        public double? Distance { get; set; } // Km
    }
}
