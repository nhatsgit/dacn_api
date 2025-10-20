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
    public class MealController : ControllerBase
    {
        private readonly _MainDbContext _context;

        public MealController(_MainDbContext context)
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

        // ✅ GET /api/meal?date=2025-10-16
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MealRecordDto>>> GetMeals([FromQuery] DateOnly? date = null)
        {
            var userId = GetUserIdFromToken();

            var query = _context.MealRecords
                .Include(m => m.MealItems)
                .ThenInclude(i => i.Food)
                .Where(m => m.UserId == userId);

            if (date.HasValue)
                query = query.Where(m => m.Date == date.Value);

            var meals = await query
                .OrderByDescending(m => m.Date)
                .Select(m => new MealRecordDto
                {
                    Id = m.Id,
                    Date = m.Date,
                    MealType = m.MealType,
                    TotalCalories = m.TotalCalories,
                    Note = m.Note,
                    Items = m.MealItems.Select(i => new MealItemDto
                    {
                        Id = i.Id,
                        FoodId = i.FoodId,
                        FoodName = i.Food != null ? i.Food.Name : null,
                        Quantity = i.Quantity,
                        Unit = i.Unit,
                        Calories = i.Calories,
                        Protein = i.Protein,
                        Carbs = i.Carbs,
                        Fat = i.Fat
                    }).ToList()
                }).ToListAsync();

            return Ok(meals);
        }

        // ✅ GET /api/meal/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<MealRecordDto>> GetMealById(int id)
        {
            var userId = GetUserIdFromToken();

            var meal = await _context.MealRecords
                .Include(m => m.MealItems)
                .ThenInclude(i => i.Food)
                .Where(m => m.Id == id && m.UserId == userId)
                .FirstOrDefaultAsync();

            if (meal == null) return NotFound();

            return Ok(new MealRecordDto
            {
                Id = meal.Id,
                Date = meal.Date,
                MealType = meal.MealType,
                TotalCalories = meal.TotalCalories,
                Note = meal.Note,
                Items = meal.MealItems.Select(i => new MealItemDto
                {
                    Id = i.Id,
                    FoodId = i.FoodId,
                    FoodName = i.Food?.Name,
                    Quantity = i.Quantity,
                    Unit = i.Unit,
                    Calories = i.Calories,
                    Protein = i.Protein,
                    Carbs = i.Carbs,
                    Fat = i.Fat
                }).ToList()
            });
        }

        // ✅ POST /api/meal
        [HttpPost]
        public async Task<ActionResult> CreateMeal([FromBody] CreateMealRecordDto dto)
        {
            var userId = GetUserIdFromToken();

            var meal = new MealRecord
            {
                UserId = userId,
                Date = dto.Date,
                MealType = dto.MealType,
                Note = dto.Note
            };

            if (dto.Items != null && dto.Items.Count > 0)
            {
                foreach (var i in dto.Items)
                {
                    var item = new MealItem
                    {
                        FoodId = i.FoodId,
                        Quantity = i.Quantity,
                        Unit = i.Unit,
                        Calories = i.Calories,
                        Protein = i.Protein,
                        Carbs = i.Carbs,
                        Fat = i.Fat
                    };
                    meal.MealItems.Add(item);
                }
                meal.TotalCalories = meal.MealItems.Sum(x => x.Calories ?? 0);
            }

            _context.MealRecords.Add(meal);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMealById), new { id = meal.Id }, meal);
        }

        // ✅ PUT /api/meal/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMeal(int id, [FromBody] CreateMealRecordDto dto)
        {
            var userId = GetUserIdFromToken();

            var meal = await _context.MealRecords
                .Include(m => m.MealItems)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (meal == null) return NotFound();

            meal.Date = dto.Date;
            meal.MealType = dto.MealType;
            meal.Note = dto.Note;

            // Xóa các item cũ
            _context.MealItems.RemoveRange(meal.MealItems);
            meal.MealItems.Clear();

            // Thêm lại item mới
            if (dto.Items != null)
            {
                foreach (var i in dto.Items)
                {
                    meal.MealItems.Add(new MealItem
                    {
                        FoodId = i.FoodId,
                        Quantity = i.Quantity,
                        Unit = i.Unit,
                        Calories = i.Calories,
                        Protein = i.Protein,
                        Carbs = i.Carbs,
                        Fat = i.Fat
                    });
                }
            }

            meal.TotalCalories = meal.MealItems.Sum(x => x.Calories ?? 0);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ✅ DELETE /api/meal/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMeal(int id)
        {
            var userId = GetUserIdFromToken();
            var meal = await _context.MealRecords
                .Include(m => m.MealItems)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (meal == null) return NotFound();

            _context.MealItems.RemoveRange(meal.MealItems);
            _context.MealRecords.Remove(meal);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
