
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
    public class WaterController : ControllerBase
    {
        private readonly _MainDbContext _context;

        public WaterController(_MainDbContext context)
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

        // ✅ GET /api/water?pageNumber=&pageSize=
        [HttpGet]
        public async Task<ActionResult<object>> GetWaterRecords(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateOnly? date = null)
        {
            var userId = GetUserIdFromToken();

            var query = _context.WaterIntakeRecords
                .Where(x => x.UserId == userId)
                .AsQueryable();

            // 🔹 Nếu có truyền ngày → chỉ lấy đúng ngày đó
            if (date.HasValue)
                query = query.Where(x => x.Date == date.Value);

            query = query.OrderByDescending(x => x.Date)
                         .ThenByDescending(x => x.Time);

            int total = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(total / (double)pageSize);

            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new WaterRecordDto
                {
                    Id = x.Id,
                    Date = x.Date,
                    Time = x.Time,
                    Amount = x.Amount,
                    Target = x.Target
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


        // ✅ GET /api/water/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<WaterRecordDto>> GetWaterRecordById(int id)
        {
            var userId = GetUserIdFromToken();

            var record = await _context.WaterIntakeRecords
                .Where(x => x.Id == id && x.UserId == userId)
                .Select(x => new WaterRecordDto
                {
                    Id = x.Id,
                    Date = x.Date,
                    Time = x.Time,
                    Amount = x.Amount,
                    Target = x.Target
                })
                .FirstOrDefaultAsync();

            if (record == null) return NotFound();

            return Ok(record);
        }

        // ✅ POST /api/water
        [HttpPost]
        public async Task<ActionResult> CreateWaterRecord([FromBody] CreateWaterRecordDto dto)
        {
            var userId = GetUserIdFromToken();

            var record = new WaterIntakeRecord
            {
                UserId = userId,
                Date = dto.Date,
                Time = dto.Time ?? DateTime.Now,
                Amount = dto.Amount,
                Target = dto.Target
            };

            _context.WaterIntakeRecords.Add(record);
            await _context.SaveChangesAsync();

            var result = new WaterRecordDto
            {
                Id = record.Id,
                Date = record.Date,
                Time = record.Time,
                Amount = record.Amount,
                Target = record.Target
            };

            return CreatedAtAction(nameof(GetWaterRecordById), new { id = record.Id }, result);
        }

        // ✅ PUT /api/water/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWaterRecord(int id, [FromBody] CreateWaterRecordDto dto)
        {
            var userId = GetUserIdFromToken();

            var record = await _context.WaterIntakeRecords.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
            if (record == null)
                return NotFound();

            record.Date = dto.Date;
            record.Time = dto.Time ?? record.Time;
            record.Amount = dto.Amount;
            record.Target = dto.Target;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ✅ DELETE /api/water/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWaterRecord(int id)
        {
            var userId = GetUserIdFromToken();

            var record = await _context.WaterIntakeRecords.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
            if (record == null)
                return NotFound();

            _context.WaterIntakeRecords.Remove(record);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
