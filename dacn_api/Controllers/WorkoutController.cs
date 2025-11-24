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
    public class WorkoutController : ControllerBase
    {
        private readonly _MainDbContext _context;
        public WorkoutController(_MainDbContext context) => _context = context;

        private Guid GetUserId() =>
            Guid.Parse(User.FindFirstValue("userId") ?? throw new Exception("Missing userId"));

        // ✅ GET /api/workout
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkoutPlanDto>>> GetPlans()
        {
            var userId = GetUserId();
            var plans = await _context.WorkoutPlans
                .Include(p => p.WorkoutPlanExercises)
                .ThenInclude(e => e.Exercise)
                .Where(p => p.UserId == userId)
                .Select(p => new WorkoutPlanDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Frequency = p.Frequency,
                    TargetSteps = p.TargetSteps,
                    PreferredTime = p.PreferredTime,
                    Notes = p.Notes,
                    Exercises = p.WorkoutPlanExercises.Select(e => new WorkoutExerciseDto
                    {
                        Id = e.Id,
                        ExerciseId = e.ExerciseId,
                        ExerciseName = e.Exercise.Name,
                        DurationMinutes = e.DurationMinutes,
                        Sets = e.Sets,
                        Reps = e.Reps,
                        DayOfWeek = e.DayOfWeek,
                        Notes = e.Notes
                    }).ToList()
                }).ToListAsync();

            return Ok(plans);
        }

        // ✅ GET /api/workout/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<WorkoutPlanDto>> GetById(int id)
        {
            var userId = GetUserId();
            var plan = await _context.WorkoutPlans
                .Include(p => p.WorkoutPlanExercises)
                .ThenInclude(e => e.Exercise)
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (plan == null) return NotFound();

            return Ok(new WorkoutPlanDto
            {
                Id = plan.Id,
                Name = plan.Name,
                Frequency = plan.Frequency,
                TargetSteps = plan.TargetSteps,
                PreferredTime = plan.PreferredTime,
                Notes = plan.Notes,
                Exercises = plan.WorkoutPlanExercises.Select(e => new WorkoutExerciseDto
                {
                    Id = e.Id,
                    ExerciseId = e.ExerciseId,
                    ExerciseName = e.Exercise.Name,
                    DurationMinutes = e.DurationMinutes,
                    Sets = e.Sets,
                    Reps = e.Reps,
                    DayOfWeek = e.DayOfWeek,
                    Notes = e.Notes
                }).ToList()
            });
        }

        // ✅ POST /api/workout
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateWorkoutPlanDto dto)
        {
            var userId = GetUserId();

            var plan = new WorkoutPlan
            {
                UserId = userId,
                Name = dto.Name,
                Frequency = dto.Frequency,
                TargetSteps = dto.TargetSteps,
                PreferredTime = dto.PreferredTime,
                Notes = dto.Notes
            };

            if (dto.Exercises != null)
            {
                foreach (var ex in dto.Exercises)
                {
                    plan.WorkoutPlanExercises.Add(new WorkoutPlanExercise
                    {
                        ExerciseId = ex.ExerciseId,
                        DurationMinutes = ex.DurationMinutes,
                        Sets = ex.Sets,
                        Reps = ex.Reps,
                        DayOfWeek = ex.DayOfWeek,
                        Notes = ex.Notes
                    });
                }
            }

            _context.WorkoutPlans.Add(plan);
            await _context.SaveChangesAsync();

            return Ok("Thành công");
        }

        // ✅ PUT /api/workout/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateWorkoutPlanDto dto)
        {
            var userId = GetUserId();
            var plan = await _context.WorkoutPlans
                .Include(p => p.WorkoutPlanExercises)
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
            if (plan == null) return NotFound();

            plan.Name = dto.Name;
            plan.Frequency = dto.Frequency;
            plan.TargetSteps = dto.TargetSteps;
            plan.PreferredTime = dto.PreferredTime;
            plan.Notes = dto.Notes;

            _context.WorkoutPlanExercises.RemoveRange(plan.WorkoutPlanExercises);
            plan.WorkoutPlanExercises.Clear();

            if (dto.Exercises != null)
            {
                foreach (var ex in dto.Exercises)
                {
                    plan.WorkoutPlanExercises.Add(new WorkoutPlanExercise
                    {
                        ExerciseId = ex.ExerciseId,
                        DurationMinutes = ex.DurationMinutes,
                        Sets = ex.Sets,
                        Reps = ex.Reps,
                        DayOfWeek = ex.DayOfWeek,
                        Notes = ex.Notes
                    });
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ✅ DELETE /api/workout/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var plan = await _context.WorkoutPlans
                .Include(p => p.WorkoutPlanExercises)
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
            if (plan == null) return NotFound();

            _context.WorkoutPlanExercises.RemoveRange(plan.WorkoutPlanExercises);
            _context.WorkoutPlans.Remove(plan);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
