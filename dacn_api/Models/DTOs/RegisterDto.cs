namespace dacn_api.Models.DTOs
{
    public class RegisterDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public double Height { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}