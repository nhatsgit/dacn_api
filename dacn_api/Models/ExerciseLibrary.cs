using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace dacn_api.Models;

[Table("ExerciseLibrary")]
public partial class ExerciseLibrary
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(50)]
    public string? Category { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public double? CaloriesPerMinute { get; set; }

    [StringLength(100)]
    public string? Equipment { get; set; }

    [StringLength(255)]
    public string? VideoUrl { get; set; }

    [InverseProperty("Exercise")]
    public virtual ICollection<WorkoutPlanExercise> WorkoutPlanExercises { get; set; } = new List<WorkoutPlanExercise>();
}
