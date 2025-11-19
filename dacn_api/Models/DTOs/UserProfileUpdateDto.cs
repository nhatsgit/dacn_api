// File: Models/DTOs/UserProfileUpdateDto.cs

using System.ComponentModel.DataAnnotations;

namespace dacn_api.Models.DTOs
{
    public class UserProfileUpdateDto
    {
        [StringLength(100)]
        public string? FullName { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public double? Height { get; set; }

        // Không cần latestWeight và BMI vì chúng được tính toán / cập nhật qua API khác
    }
}