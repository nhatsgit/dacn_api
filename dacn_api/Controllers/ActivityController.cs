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
    [Authorize] // Bảo vệ Controller, chỉ người dùng đã đăng nhập mới truy cập được
    public class ActivityController : ControllerBase
    {
        private readonly _MainDbContext _context;

        // Dependency Injection cho DbContext
        public ActivityController(_MainDbContext context) => _context = context;

        // Hàm tiện ích để lấy UserId từ JWT Token
        private Guid GetUserId() =>
            Guid.Parse(User.FindFirstValue("userId") ?? throw new Exception("Missing userId claim"));

        // ---------------------------------------------------------------------
        // ✅ 1. GET: Lấy danh sách hồ sơ hoạt động theo ngày
        // GET /api/activity?date=yyyy-MM-dd
        // ---------------------------------------------------------------------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActivityRecordDto>>> GetRecords(
            [FromQuery] DateOnly? date)
        {
            var userId = GetUserId();
            var query = _context.ActivityRecords
                .Where(r => r.UserId == userId);

            // Lọc theo ngày nếu có
            if (date.HasValue)
            {
                query = query.Where(r => r.Date == date.Value);
            }

            var records = await query
                .OrderByDescending(r => r.Date)
                .Select(r => new ActivityRecordDto
                {
                    Id = r.Id,
                    Date = r.Date,
                    ActivityType = r.ActivityType,
                    Duration = r.Duration,
                    CaloriesOut = r.CaloriesOut,
                    Steps = r.Steps,
                    Distance = r.Distance
                })
                .ToListAsync();

            return Ok(records);
        }

        // ---------------------------------------------------------------------
        // ✅ 2. GET: Lấy hồ sơ hoạt động theo ID
        // GET /api/activity/{id}
        // ---------------------------------------------------------------------
        [HttpGet("{id}")]
        public async Task<ActionResult<ActivityRecordDto>> GetRecord(int id)
        {
            var userId = GetUserId();
            var record = await _context.ActivityRecords
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (record == null) return NotFound();

            var dto = new ActivityRecordDto
            {
                Id = record.Id,
                Date = record.Date,
                ActivityType = record.ActivityType,
                Duration = record.Duration,
                CaloriesOut = record.CaloriesOut,
                Steps = record.Steps,
                Distance = record.Distance
            };

            return Ok(dto);
        }

        // ---------------------------------------------------------------------
        // ✅ 3. POST: Tạo hồ sơ hoạt động mới
        // POST /api/activity
        // ---------------------------------------------------------------------
        [HttpPost]
        public async Task<ActionResult<ActivityRecordDto>> CreateRecord(
            [FromBody] CreateActivityRecordDto dto)
        {
            var userId = GetUserId();

            var record = new ActivityRecord
            {
                UserId = userId,
                Date = dto.Date,
                ActivityType = dto.ActivityType,
                Duration = dto.Duration,
                CaloriesOut = dto.CaloriesOut,
                Steps = dto.Steps,
                Distance = dto.Distance
            };

            _context.ActivityRecords.Add(record);
            await _context.SaveChangesAsync();

            // Trả về DTO của bản ghi vừa tạo
            var resultDto = new ActivityRecordDto
            {
                Id = record.Id,
                Date = record.Date,
                ActivityType = record.ActivityType,
                Duration = record.Duration,
                CaloriesOut = record.CaloriesOut,
                Steps = record.Steps,
                Distance = record.Distance
            };

            return CreatedAtAction(nameof(GetRecord), new { id = record.Id }, resultDto);
        }

        // ---------------------------------------------------------------------
        // ✅ 4. PUT: Cập nhật hồ sơ hoạt động hiện có
        // PUT /api/activity/{id}
        // ---------------------------------------------------------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRecord(
            int id, [FromBody] CreateActivityRecordDto dto)
        {
            var userId = GetUserId();

            var record = await _context.ActivityRecords
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (record == null) return NotFound();

            // Cập nhật các trường
            record.Date = dto.Date;
            record.ActivityType = dto.ActivityType;
            record.Duration = dto.Duration;
            record.CaloriesOut = dto.CaloriesOut;
            record.Steps = dto.Steps;
            record.Distance = dto.Distance;

            await _context.SaveChangesAsync();

            // Trả về 204 No Content cho thao tác cập nhật thành công (theo chuẩn REST)
            return NoContent();
        }

        // ---------------------------------------------------------------------
        // ✅ 5. DELETE: Xóa hồ sơ hoạt động
        // DELETE /api/activity/{id}
        // ---------------------------------------------------------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecord(int id)
        {
            var userId = GetUserId();
            var record = await _context.ActivityRecords
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (record == null) return NotFound();

            _context.ActivityRecords.Remove(record);
            await _context.SaveChangesAsync();

            // Trả về 204 No Content cho thao tác xóa thành công (theo chuẩn REST)
            return NoContent();
        }
    }
}