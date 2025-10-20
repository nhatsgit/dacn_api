using dacn_api.EF;
using dacn_api.Models;
using dacn_api.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dacn_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodController : ControllerBase
    {
        private readonly _MainDbContext _context;

        public FoodController(_MainDbContext context)
        {
            _context = context;
        }

        // ✅ GET /api/food
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FoodDto>>> GetAll()
        {
            var data = await _context.FoodDatabases
                .Select(f => new FoodDto
                {
                    Id = f.Id,
                    Name = f.Name,
                    Barcode = f.Barcode,
                    Calories = f.Calories,
                    Protein = f.Protein,
                    Carbs = f.Carbs,
                    Fat = f.Fat,
                    ServingSize = f.ServingSize,
                    Type = f.Type,
                    Instructions = f.Instructions
                }).ToListAsync();
            return Ok(data);
        }

        // ✅ GET /api/food/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<FoodDto>> GetById(int id)
        {
            var f = await _context.FoodDatabases.FindAsync(id);
            if (f == null) return NotFound();
            return Ok(new FoodDto
            {
                Id = f.Id,
                Name = f.Name,
                Barcode = f.Barcode,
                Calories = f.Calories,
                Protein = f.Protein,
                Carbs = f.Carbs,
                Fat = f.Fat,
                ServingSize = f.ServingSize,
                Type = f.Type,
                Instructions = f.Instructions
            });
        }

        // ✅ POST /api/food
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateFoodDto dto)
        {
            var f = new FoodDatabase
            {
                Name = dto.Name,
                Barcode = dto.Barcode,
                Calories = dto.Calories,
                Protein = dto.Protein,
                Carbs = dto.Carbs,
                Fat = dto.Fat,
                ServingSize = dto.ServingSize,
                Type = dto.Type,
                Instructions = dto.Instructions
            };
            _context.FoodDatabases.Add(f);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = f.Id }, f);
        }

        // ✅ PUT /api/food/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateFoodDto dto)
        {
            var f = await _context.FoodDatabases.FindAsync(id);
            if (f == null) return NotFound();

            f.Name = dto.Name;
            f.Barcode = dto.Barcode;
            f.Calories = dto.Calories;
            f.Protein = dto.Protein;
            f.Carbs = dto.Carbs;
            f.Fat = dto.Fat;
            f.ServingSize = dto.ServingSize;
            f.Type = dto.Type;
            f.Instructions = dto.Instructions;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ✅ DELETE /api/food/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var f = await _context.FoodDatabases.FindAsync(id);
            if (f == null) return NotFound();
            _context.FoodDatabases.Remove(f);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
