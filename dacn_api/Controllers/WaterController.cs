
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
        public async Task<ActionResult<WaterRecordDto>> CreateWaterRecord([FromBody] CreateWaterRecordDto dto)
        {
            var userId = GetUserIdFromToken();

            // 💡 1. CHUẨN BỊ NGÀY CẦN XỬ LÝ (Sử dụng ngày từ DTO)
            var targetDate = dto.Date; // DateOnly từ DTO

            // 💡 2. TÌM BẢN GHI ĐÃ TỒN TẠI TRONG NGÀY
            var existingRecord = await _context.WaterIntakeRecords
                .FirstOrDefaultAsync(r => r.UserId == userId && r.Date == targetDate);

            if (existingRecord != null)
            {
                // 🔹 TRƯỜNG HỢP 2: ĐÃ CÓ BẢN GHI → UPDATE CỘNG THÊM
                existingRecord.Amount += dto.Amount;

                // Cập nhật các trường tùy chọn nếu có trong DTO
                if (dto.Time.HasValue)
                {
                    existingRecord.Time = dto.Time;
                }
                if (dto.Target.HasValue)
                {
                    existingRecord.Target = dto.Target;
                }

                _context.WaterIntakeRecords.Update(existingRecord);
                await _context.SaveChangesAsync();

                // Trả về bản ghi đã được cập nhật
                var updatedResult = new WaterRecordDto
                {
                    Id = existingRecord.Id,
                    Date = existingRecord.Date,
                    Time = existingRecord.Time,
                    Amount = existingRecord.Amount,
                    Target = existingRecord.Target
                };
                // Dùng OK (200) cho trường hợp cập nhật
                return Ok(updatedResult);
            }
            else
            {
                // 🔹 TRƯỜNG HỢP 1: CHƯA CÓ BẢN GHI → CREATE MỚI
                var newRecord = new WaterIntakeRecord
                {
                    UserId = userId,
                    Date = dto.Date,
                    Time = dto.Time,
                    Amount = dto.Amount,
                    Target = dto.Target
                };

                _context.WaterIntakeRecords.Add(newRecord);
                await _context.SaveChangesAsync();

                // Trả về bản ghi mới tạo (sử dụng CreatedAtAction hoặc tạo DTO mới)
                var newResult = new WaterRecordDto
                {
                    Id = newRecord.Id,
                    Date = newRecord.Date,
                    Time = newRecord.Time,
                    Amount = newRecord.Amount,
                    Target = newRecord.Target
                };

                return CreatedAtAction(nameof(GetWaterRecordById), new { id = newRecord.Id }, newResult);
            }
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

        // WaterController.cs

        // ... (các using, class định nghĩa)

        // ✅ GET /api/water/today
        [HttpGet("today")]
        public async Task<ActionResult<WaterRecordDto>> GetOrCreateTodayWaterRecord()
        {
            var userId = GetUserIdFromToken();
            var today = DateOnly.FromDateTime(DateTime.Today); // Lấy ngày hôm nay dưới dạng DateOnly

            // 1. Tìm bản ghi lượng nước uống cho ngày hôm nay
            var record = await _context.WaterIntakeRecords
                .Where(x => x.UserId == userId && x.Date == today)
                .OrderByDescending(x => x.Time) // Sắp xếp để đảm bảo lấy bản ghi gần nhất nếu có nhiều (thường là bản ghi đầu tiên trong ngày)
                .Select(x => new WaterRecordDto
                {
                    Id = x.Id,
                    Date = x.Date,
                    Time = x.Time,
                    Amount = x.Amount,
                    Target = x.Target
                })
                .FirstOrDefaultAsync();

            // 2. Nếu đã có bản ghi → trả về
            if (record != null)
            {
                return Ok(record);
            }

            // 3. Nếu chưa có → tạo bản ghi mới với Amount = 0
            var newRecord = new WaterIntakeRecord
            {
                UserId = userId,
                Date = today,
                Time = DateTime.Now, // Ghi lại thời điểm tạo
                Amount = 0,          // Khởi tạo lượng nước = 0
                Target = null        // Target có thể là null hoặc lấy từ một cài đặt mặc định
            };

            // 3.1. Tìm Target gần nhất (tùy chọn: bạn có thể muốn lấy target từ bản ghi cũ nhất/gần nhất nếu có)
            var lastRecord = await _context.WaterIntakeRecords
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.Date)
                .ThenByDescending(x => x.Time)
                .FirstOrDefaultAsync();

            if (lastRecord != null && lastRecord.Target.HasValue)
            {
                newRecord.Target = lastRecord.Target; // Sử dụng Target của bản ghi gần nhất
            }

            // 3.2. Thêm và lưu vào DB
            _context.WaterIntakeRecords.Add(newRecord);
            await _context.SaveChangesAsync();

            // 3.3. Trả về bản ghi mới được tạo
            var resultDto = new WaterRecordDto
            {
                Id = newRecord.Id,
                Date = newRecord.Date,
                Time = newRecord.Time,
                Amount = newRecord.Amount,
                Target = newRecord.Target
            };

            return CreatedAtAction(nameof(GetWaterRecordById), new { id = newRecord.Id }, resultDto);
        }
    }
}
