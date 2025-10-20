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
    public class MedicationsController : ControllerBase
    {
        private readonly _MainDbContext _context;

        public MedicationsController(_MainDbContext context)
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

        // ✅ GET /api/medications?active=true&date=2025-10-16&pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<ActionResult<object>> GetMedications(
            [FromQuery] bool? active = null,
            [FromQuery] DateOnly? date = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var userId = GetUserIdFromToken();

            var query = _context.MedicationReminders
                .Where(m => m.UserId == userId)
                .AsQueryable();

            if (active.HasValue)
                query = query.Where(m => m.IsActive == active.Value);

            if (date.HasValue)
                query = query.Where(m =>
                    m.StartDate <= date.Value &&
                    (m.EndDate == null || m.EndDate >= date.Value));

            query = query.OrderByDescending(m => m.StartDate);

            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new MedicationReminderDto
                {
                    Id = m.Id,
                    MedicineName = m.MedicineName,
                    Dosage = m.Dosage,
                    Frequency = m.Frequency,
                    StartDate = m.StartDate,
                    EndDate = m.EndDate,
                    ReminderTime = m.ReminderTime,
                    Note = m.Note,
                    IsActive = m.IsActive
                })
                .ToListAsync();

            return Ok(new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                ActiveFilter = active,
                DateFilter = date,
                Data = data
            });
        }

        // ✅ GET /api/medications/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<MedicationReminderDto>> GetMedicationById(int id)
        {
            var userId = GetUserIdFromToken();

            var record = await _context.MedicationReminders
                .Where(m => m.Id == id && m.UserId == userId)
                .Select(m => new MedicationReminderDto
                {
                    Id = m.Id,
                    MedicineName = m.MedicineName,
                    Dosage = m.Dosage,
                    Frequency = m.Frequency,
                    StartDate = m.StartDate,
                    EndDate = m.EndDate,
                    ReminderTime = m.ReminderTime,
                    Note = m.Note,
                    IsActive = m.IsActive
                })
                .FirstOrDefaultAsync();

            if (record == null)
                return NotFound();

            return Ok(record);
        }

        // ✅ POST /api/medications
        [HttpPost]
        public async Task<ActionResult> CreateMedication([FromBody] CreateMedicationReminderDto dto)
        {
            var userId = GetUserIdFromToken();

            var record = new MedicationReminder
            {
                UserId = userId,
                MedicineName = dto.MedicineName,
                Dosage = dto.Dosage,
                Frequency = dto.Frequency,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                ReminderTime = dto.ReminderTime,
                Note = dto.Note,
                IsActive = dto.IsActive ?? true
            };

            _context.MedicationReminders.Add(record);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMedicationById), new { id = record.Id }, record);
        }

        // ✅ PUT /api/medications/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMedication(int id, [FromBody] CreateMedicationReminderDto dto)
        {
            var userId = GetUserIdFromToken();

            var record = await _context.MedicationReminders.FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (record == null)
                return NotFound();

            record.MedicineName = dto.MedicineName;
            record.Dosage = dto.Dosage;
            record.Frequency = dto.Frequency;
            record.StartDate = dto.StartDate;
            record.EndDate = dto.EndDate;
            record.ReminderTime = dto.ReminderTime;
            record.Note = dto.Note;
            record.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ✅ DELETE /api/medications/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedication(int id)
        {
            var userId = GetUserIdFromToken();

            var record = await _context.MedicationReminders.FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (record == null)
                return NotFound();

            _context.MedicationReminders.Remove(record);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
