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
    public class SleepController : ControllerBase
    {
        private readonly _MainDbContext _context;

        public SleepController(_MainDbContext context)
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

        // ✅ GET /api/sleep?date=&pageNumber=&pageSize=
        [HttpGet]
        public async Task<ActionResult<object>> GetSleepRecords(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateOnly? date = null)
        {
            var userId = GetUserIdFromToken();

            var query = _context.SleepRecords
                .Where(s => s.UserId == userId)
                .AsQueryable();

            // Nếu truyền ngày → lọc theo ngày bắt đầu
            if (date.HasValue)
            {
                var startDate = date.Value.ToDateTime(TimeOnly.MinValue);
                var endDate = date.Value.ToDateTime(TimeOnly.MaxValue);
                query = query.Where(s => s.StartTime >= startDate && s.StartTime <= endDate);
            }

            query = query.OrderByDescending(s => s.StartTime);

            int total = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(total / (double)pageSize);

            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new SleepRecordDto
                {
                    Id = s.Id,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    DurationMinutes = s.DurationMinutes,
                    SleepQuality = s.SleepQuality,
                    SleepType = s.SleepType,
                    Notes = s.Notes
                })
                .ToListAsync();

            return Ok(new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = total,
                TotalPages = totalPages,
                FilterDate = date,
                Data = data
            });
        }

        // ✅ GET /api/sleep/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<SleepRecordDto>> GetSleepRecordById(int id)
        {
            var userId = GetUserIdFromToken();

            var record = await _context.SleepRecords
                .Where(s => s.Id == id && s.UserId == userId)
                .Select(s => new SleepRecordDto
                {
                    Id = s.Id,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    DurationMinutes = s.DurationMinutes,
                    SleepQuality = s.SleepQuality,
                    SleepType = s.SleepType,
                    Notes = s.Notes
                })
                .FirstOrDefaultAsync();

            if (record == null)
                return NotFound();

            return Ok(record);
        }

        // ✅ POST /api/sleep
        [HttpPost]
        public async Task<ActionResult> CreateSleepRecord([FromBody] CreateSleepRecordDto dto)
        {
            var userId = GetUserIdFromToken();

            if (dto.EndTime <= dto.StartTime)
                return BadRequest("EndTime must be after StartTime.");

            int duration = (int)(dto.EndTime - dto.StartTime).TotalMinutes;

            var record = new SleepRecord
            {
                UserId = userId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                DurationMinutes = duration,
                SleepQuality = dto.SleepQuality,
                SleepType = dto.SleepType,
                Notes = dto.Notes
            };

            _context.SleepRecords.Add(record);
            await _context.SaveChangesAsync();

            var result = new SleepRecordDto
            {
                Id = record.Id,
                StartTime = record.StartTime,
                EndTime = record.EndTime,
                DurationMinutes = record.DurationMinutes,
                SleepQuality = record.SleepQuality,
                SleepType = record.SleepType,
                Notes = record.Notes
            };

            return CreatedAtAction(nameof(GetSleepRecordById), new { id = record.Id }, result);
        }

        // ✅ PUT /api/sleep/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSleepRecord(int id, [FromBody] CreateSleepRecordDto dto)
        {
            var userId = GetUserIdFromToken();

            var record = await _context.SleepRecords.FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);
            if (record == null)
                return NotFound();

            if (dto.EndTime <= dto.StartTime)
                return BadRequest("EndTime must be after StartTime.");

            record.StartTime = dto.StartTime;
            record.EndTime = dto.EndTime;
            record.DurationMinutes = (int)(dto.EndTime - dto.StartTime).TotalMinutes;
            record.SleepQuality = dto.SleepQuality;
            record.SleepType = dto.SleepType;
            record.Notes = dto.Notes;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ✅ DELETE /api/sleep/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSleepRecord(int id)
        {
            var userId = GetUserIdFromToken();

            var record = await _context.SleepRecords.FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);
            if (record == null)
                return NotFound();

            _context.SleepRecords.Remove(record);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
