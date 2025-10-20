using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace dacn_api.Models;

public partial class DailyGoal
{
    [Key]
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public DateOnly GoalDate { get; set; }

    [StringLength(50)]
    public string GoalType { get; set; } = null!;

    public double TargetValue { get; set; }

    public double? CurrentValue { get; set; }

    [StringLength(20)]
    public string? Unit { get; set; }

    public int IsCompleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastUpdated { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("DailyGoals")]
    public virtual User User { get; set; } = null!;
}
