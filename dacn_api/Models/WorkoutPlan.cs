using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace dacn_api.Models;

public partial class WorkoutPlan
{
    [Key]
    public int Id { get; set; }

    public Guid UserId { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(20)]
    public string? Frequency { get; set; }

    public int? TargetSteps { get; set; }

    public TimeOnly? PreferredTime { get; set; }

    [StringLength(200)]
    public string? Notes { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("WorkoutPlans")]
    public virtual User User { get; set; } = null!;

    [InverseProperty("WorkoutPlan")]
    public virtual ICollection<WorkoutPlanExercise> WorkoutPlanExercises { get; set; } = new List<WorkoutPlanExercise>();
}
