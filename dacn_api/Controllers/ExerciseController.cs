using dacn_api.EF;
using dacn_api.Models;
using dacn_api.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dacn_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExerciseController : ControllerBase
    {
        private readonly _MainDbContext _context;
        public ExerciseController(_MainDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExerciseDto>>> GetAll()
        {
            var data = await _context.ExerciseLibraries
                .Select(e => new ExerciseDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Category = e.Category,
                    Description = e.Description,
                    CaloriesPerMinute = e.CaloriesPerMinute,
                    Equipment = e.Equipment,
                    VideoUrl = e.VideoUrl
                }).ToListAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ExerciseDto>> GetById(int id)
        {
            var e = await _context.ExerciseLibraries.FindAsync(id);
            if (e == null) return NotFound();
            return Ok(new ExerciseDto
            {
                Id = e.Id,
                Name = e.Name,
                Category = e.Category,
                Description = e.Description,
                CaloriesPerMinute = e.CaloriesPerMinute,
                Equipment = e.Equipment,
                VideoUrl = e.VideoUrl
            });
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateExerciseDto dto)
        {
            var e = new ExerciseLibrary
            {
                Name = dto.Name,
                Category = dto.Category,
                Description = dto.Description,
                CaloriesPerMinute = dto.CaloriesPerMinute,
                Equipment = dto.Equipment,
                VideoUrl = dto.VideoUrl
            };
            _context.ExerciseLibraries.Add(e);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = e.Id }, e);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateExerciseDto dto)
        {
            var e = await _context.ExerciseLibraries.FindAsync(id);
            if (e == null) return NotFound();
            e.Name = dto.Name;
            e.Category = dto.Category;
            e.Description = dto.Description;
            e.CaloriesPerMinute = dto.CaloriesPerMinute;
            e.Equipment = dto.Equipment;
            e.VideoUrl = dto.VideoUrl;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var e = await _context.ExerciseLibraries.FindAsync(id);
            if (e == null) return NotFound();
            _context.ExerciseLibraries.Remove(e);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
