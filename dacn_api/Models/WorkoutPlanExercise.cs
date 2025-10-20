using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace dacn_api.Models;

[Index("ExerciseId", Name = "IX_WorkoutPlanExercises_ExerciseId")]
[Index("WorkoutPlanId", Name = "IX_WorkoutPlanExercises_PlanId")]
public partial class WorkoutPlanExercise
{
    [Key]
    public int Id { get; set; }

    public int WorkoutPlanId { get; set; }

    public int ExerciseId { get; set; }

    public int? DurationMinutes { get; set; }

    public int? Sets { get; set; }

    public int? Reps { get; set; }

    [StringLength(20)]
    public string? DayOfWeek { get; set; }

    [StringLength(200)]
    public string? Notes { get; set; }

    [ForeignKey("ExerciseId")]
    [InverseProperty("WorkoutPlanExercises")]
    public virtual ExerciseLibrary Exercise { get; set; } = null!;

    [ForeignKey("WorkoutPlanId")]
    [InverseProperty("WorkoutPlanExercises")]
    public virtual WorkoutPlan WorkoutPlan { get; set; } = null!;
}
