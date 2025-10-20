using dacn_api.EF;
using dacn_api.Models;
using dacn_api.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace dacn_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly _MainDbContext _context;

        public UserController(_MainDbContext context)
        {
            _context = context;
        }

        private Guid GetUserIdFromToken()
        {
            var claim = User.FindFirst("userId");
            if (claim == null)
                throw new Exception("Missing userId in token.");
            return Guid.Parse(claim.Value);
        }

        // ✅ 1. Lấy thông tin người dùng
        [HttpGet("profile")]
        public async Task<ActionResult<UserProfileDto>> GetProfile()
        {
            var userId = GetUserIdFromToken();

            var user = await _context.Users
                .Include(u => u.WeightRecords)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound("User not found.");

            var latestWeight = user.WeightRecords
                .OrderByDescending(w => w.Date)
                .Select(w => w.Weight)
                .FirstOrDefault();

            double? bmi = null;
            if (user.Height.HasValue && latestWeight > 0)
            {
                double h = user.Height.Value / 100.0;
                bmi = Math.Round(latestWeight / (h * h), 2);
            }

            return Ok(new UserProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,
                Height = user.Height,
                LatestWeight = latestWeight,
                Bmi = bmi
            });
        }

        // ✅ 2. Lấy cân nặng lý tưởng
        [HttpGet("ideal-weight")]
        public async Task<ActionResult<double>> GetIdealWeight()
        {
            var userId = GetUserIdFromToken();

            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.Height == null)
                return BadRequest("Missing height.");

            double heightM = user.Height.Value / 100.0;
            double idealWeight = 22 * heightM * heightM;

            return Ok(Math.Round(idealWeight, 2));
        }

        // ✅ 3. Lấy lượng nước lý tưởng = cân nặng gần nhất × 40 (ml)
        [HttpGet("ideal-water")]
        public async Task<ActionResult<double>> GetIdealWater()
        {
            var userId = GetUserIdFromToken();

            var latestWeight = await _context.WeightRecords
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.Date)
                .Select(w => w.Weight)
                .FirstOrDefaultAsync();

            if (latestWeight <= 0)
                return BadRequest("Missing weight record.");

            double water = latestWeight * 40; // ml
            return Ok(Math.Round(water, 0)); // ví dụ 2600 ml
        }

        // ✅ 4. Lấy calo nạp lý tưởng (TDEE ~ 30 kcal/kg)
        [HttpGet("ideal-calories-in")]
        public async Task<ActionResult<double>> GetIdealCaloriesIn()
        {
            var userId = GetUserIdFromToken();

            var latestWeight = await _context.WeightRecords
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.Date)
                .Select(w => w.Weight)
                .FirstOrDefaultAsync();

            if (latestWeight <= 0)
                return BadRequest("Missing weight record.");

            double caloriesIn = latestWeight * 30; // kcal
            return Ok(Math.Round(caloriesIn, 0));  // ví dụ 1980 kcal
        }

        // ✅ 5. Lấy calo đốt lý tưởng (TDEE × 0.15 để giảm cân nhẹ)
        [HttpGet("ideal-calories-out")]
        public async Task<ActionResult<double>> GetIdealCaloriesOut()
        {
            var userId = GetUserIdFromToken();

            var latestWeight = await _context.WeightRecords
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.Date)
                .Select(w => w.Weight)
                .FirstOrDefaultAsync();

            if (latestWeight <= 0)
                return BadRequest("Missing weight record.");

            double caloriesOut = latestWeight * 30 * 0.15; // 15% TDEE burn target
            return Ok(Math.Round(caloriesOut, 0)); // ví dụ 300 kcal/ngày
        }

        // ✅ (Tuỳ chọn) endpoint tổng hợp cho dashboard
        [HttpGet("summary")]
        public async Task<ActionResult<IdealStatsDto>> GetSummary()
        {
            var userId = GetUserIdFromToken();

            var user = await _context.Users.Include(u => u.WeightRecords)
                                           .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null || user.Height == null)
                return BadRequest("Missing user or height.");

            var latestWeight = user.WeightRecords.OrderByDescending(w => w.Date)
                                                 .Select(w => w.Weight)
                                                 .FirstOrDefault();

            double heightM = user.Height.Value / 100.0;
            double idealWeight = 22 * heightM * heightM;
            double idealWater = latestWeight * 40;
            double idealCaloriesIn = latestWeight * 30;
            double idealCaloriesOut = latestWeight * 30 * 0.15;

            return Ok(new IdealStatsDto
            {
                IdealWeight = Math.Round(idealWeight, 2),
                IdealWaterMl = Math.Round(idealWater, 0),
                IdealCaloriesIn = Math.Round(idealCaloriesIn, 0),
                IdealCaloriesOut = Math.Round(idealCaloriesOut, 0)
            });
        }
    }
}
