using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace dacn_api.Models;

public partial class UserGoal
{
    [Key]
    public int Id { get; set; }

    public Guid UserId { get; set; }

    [StringLength(50)]
    public string GoalType { get; set; } = null!;

    public double TargetValue { get; set; }

    [StringLength(20)]
    public string? Unit { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public bool? IsActive { get; set; }

    [StringLength(200)]
    public string? Note { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserGoals")]
    public virtual User User { get; set; } = null!;
}
