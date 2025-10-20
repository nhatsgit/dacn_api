namespace dacn_api.Models.DTOs
{
    public class ExerciseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Category { get; set; }
        public string? Description { get; set; }
        public double? CaloriesPerMinute { get; set; }
        public string? Equipment { get; set; }
        public string? VideoUrl { get; set; }
    }

    public class CreateExerciseDto
    {
        public string Name { get; set; } = null!;
        public string? Category { get; set; }
        public string? Description { get; set; }
        public double? CaloriesPerMinute { get; set; }
        public string? Equipment { get; set; }
        public string? VideoUrl { get; set; }
    }

    public class WorkoutExerciseDto
    {
        public int Id { get; set; }
        public int ExerciseId { get; set; }
        public string? ExerciseName { get; set; }
        public int? DurationMinutes { get; set; }
        public int? Sets { get; set; }
        public int? Reps { get; set; }
        public string? DayOfWeek { get; set; }
        public string? Notes { get; set; }
    }

    public class CreateWorkoutExerciseDto
    {
        public int ExerciseId { get; set; }
        public int? DurationMinutes { get; set; }
        public int? Sets { get; set; }
        public int? Reps { get; set; }
        public string? DayOfWeek { get; set; }
        public string? Notes { get; set; }
    }

    public class WorkoutPlanDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Frequency { get; set; }
        public int? TargetSteps { get; set; }
        public TimeOnly? PreferredTime { get; set; }
        public string? Notes { get; set; }
        public List<WorkoutExerciseDto>? Exercises { get; set; }
    }

    public class CreateWorkoutPlanDto
    {
        public string Name { get; set; } = null!;
        public string? Frequency { get; set; }
        public int? TargetSteps { get; set; }
        public TimeOnly? PreferredTime { get; set; }
        public string? Notes { get; set; }
        public List<CreateWorkoutExerciseDto>? Exercises { get; set; }
    }

}
