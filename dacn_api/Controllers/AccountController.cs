using dacn_api.EF;
using dacn_api.Models;
using dacn_api.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace dacn_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<Account> _userManager;
        private readonly _MainDbContext _context;

        private readonly IConfiguration _configuration;
        public AccountController(UserManager<Account> userManager, _MainDbContext context, IConfiguration configuration)
        {
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (await _userManager.Users.AnyAsync(u => u.Email == model.Email))
                return BadRequest("Email đã tồn tại");

            // 1️⃣ Tạo bản ghi User
            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = model.FullName,
                Gender = model.Gender,
                Height=model.Height,
                DateOfBirth = model.DateOfBirth,
                CreatedDate = DateTime.Now
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // 2️⃣ Tạo Account liên kết với User
            var account = new Account
            {
                UserName = model.Email,
                Email = model.Email,
                UserId = user.Id
            };

            var result = await _userManager.CreateAsync(account, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Đăng ký thành công", userId = user.Id });
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var account = await _userManager.FindByEmailAsync(model.Email);
            if (account == null)
                return Unauthorized(new { message = "Email không tồn tại" });

            var valid = await _userManager.CheckPasswordAsync(account, model.Password);
            if (!valid)
                return Unauthorized(new { message = "Sai mật khẩu" });

            // Tạo JWT Token
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, account.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, account.Email ?? ""),
        new Claim("userId", account.UserId?.ToString() ?? "")
    };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(double.Parse(jwtSettings["ExpiresInHours"])),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(tokenString);
        }
    }
}
