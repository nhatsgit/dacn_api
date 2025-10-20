
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
    [Authorize] // ✅ yêu cầu JWT
    public class WeightsController : ControllerBase
    {
        private readonly _MainDbContext _context;

        public WeightsController(_MainDbContext context)
        {
            _context = context;
        }

        // ✅ Lấy userId từ JWT
        private Guid GetUserIdFromToken()
        {
            var claim = User.FindFirst("userId");
            if (claim == null) throw new Exception("Missing userId in token.");
            return Guid.Parse(claim.Value);
        }

        // ✅ GET /api/weights?date=&pageNumber=&pageSize=
        [HttpGet]
        public async Task<ActionResult<object>> GetWeights(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateOnly? date = null)
        {
            var userId = GetUserIdFromToken();

            var query = _context.WeightRecords
                .Where(w => w.UserId == userId)
                .AsQueryable();

            // 🔹 Nếu có truyền ngày → lọc theo ngày đó
            if (date.HasValue)
                query = query.Where(w => w.Date == date.Value);

            query = query.OrderByDescending(w => w.Date);

            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(w => new WeightRecordDto
                {
                    Id = w.Id,
                    Date = w.Date,
                    Weight = w.Weight,
                    Bmi = w.Bmi,
                    IdealWeight = w.IdealWeight,
                    Note = w.Note
                })
                .ToListAsync();

            return Ok(new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                FilterDate = date,
                Data = data
            });
        }



        // ✅ GET /api/weights/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<WeightRecordDto>> GetWeightById(int id)
        {
            var userId = GetUserIdFromToken();

            var record = await _context.WeightRecords
                .Where(w => w.Id == id && w.UserId == userId)
                .Select(x => new WeightRecordDto
                {
                    Id = x.Id,
                    Date = x.Date,
                    Weight = x.Weight,
                    Bmi = x.Bmi,
                    IdealWeight = x.IdealWeight,
                    Note = x.Note
                })
                .FirstOrDefaultAsync();

            if (record == null)
                return NotFound();

            return Ok(record);
        }

        // ✅ POST /api/weights
        [HttpPost]
        public async Task<ActionResult> CreateWeight([FromBody] CreateWeightRecordDto dto)
        {
            var userId = GetUserIdFromToken();

            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.Height == null)
                return BadRequest("User not found or missing height.");

            double heightM = user.Height.Value / 100.0;
            double bmi = dto.Weight / (heightM * heightM);


            var record = new WeightRecord
            {
                UserId = userId,
                Date = dto.Date,
                Weight = dto.Weight,
                Bmi = Math.Round(bmi, 2),
                Note = dto.Note
            };

            _context.WeightRecords.Add(record);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetWeightById), new { id = record.Id }, new WeightRecordDto
            {
                Id = record.Id,
                Date = record.Date,
                Weight = record.Weight,
                Bmi = record.Bmi,
                IdealWeight = record.IdealWeight,
                Note = record.Note
            });
        }

        // ✅ PUT /api/weights/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWeight(int id, [FromBody] CreateWeightRecordDto dto)
        {
            var userId = GetUserIdFromToken();

            var record = await _context.WeightRecords.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
            if (record == null) return NotFound();

            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.Height == null)
                return BadRequest("User not found or missing height.");

            double heightM = user.Height.Value / 100.0;
            double bmi = dto.Weight / (heightM * heightM);
            double idealWeight = 22 * heightM * heightM;

            record.Weight = dto.Weight;
            record.Bmi = Math.Round(bmi, 2);
            record.IdealWeight = Math.Round(idealWeight, 2);
            record.Date = dto.Date;
            record.Note = dto.Note;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ✅ DELETE /api/weights/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWeight(int id)
        {
            var userId = GetUserIdFromToken();

            var record = await _context.WeightRecords.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
            if (record == null) return NotFound();

            _context.WeightRecords.Remove(record);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        //// ✅ GET /api/weights/summary?period=daily/weekly/monthly
        //[HttpGet("summary")]
        //public async Task<IActionResult> GetSummary([FromQuery] string period = "daily")
        //{
        //    var userId = GetUserIdFromToken();

        //    var records = _context.WeightRecords.Where(w => w.UserId == userId);

        //    var grouped = period.ToLower() switch
        //    {
        //        "weekly" => records.GroupBy(w => EF.Functions.DatePart("week", w.Date)),
        //        "monthly" => records.GroupBy(w => w.Date.Month),
        //        _ => records.GroupBy(w => w.Date)
        //    };

        //    var result = await grouped.Select(g => new
        //    {
        //        Period = g.Key,
        //        AvgWeight = g.Average(x => x.Weight),
        //        AvgBmi = g.Average(x => x.Bmi),
        //        Count = g.Count()
        //    }).ToListAsync();

        //    return Ok(result);
        //}
    }
}
