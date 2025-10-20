using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace dacn_api.Models;

public partial class User
{
    [Key]
    public Guid Id { get; set; }

    [StringLength(100)]
    public string? FullName { get; set; }

    [StringLength(10)]
    public string? Gender { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DateOfBirth { get; set; }

    public double? Height { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<ActivityRecord> ActivityRecords { get; set; } = new List<ActivityRecord>();

    [InverseProperty("User")]
    public virtual ICollection<DailyGoal> DailyGoals { get; set; } = new List<DailyGoal>();

    [InverseProperty("User")]
    public virtual ICollection<HealthMetric> HealthMetrics { get; set; } = new List<HealthMetric>();

    [InverseProperty("User")]
    public virtual ICollection<MealRecord> MealRecords { get; set; } = new List<MealRecord>();

    [InverseProperty("User")]
    public virtual ICollection<MedicationReminder> MedicationReminders { get; set; } = new List<MedicationReminder>();

    [InverseProperty("User")]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    [InverseProperty("User")]
    public virtual ICollection<SleepRecord> SleepRecords { get; set; } = new List<SleepRecord>();

    [InverseProperty("User")]
    public virtual ICollection<UserGoal> UserGoals { get; set; } = new List<UserGoal>();

    [InverseProperty("User")]
    public virtual ICollection<WaterIntakeRecord> WaterIntakeRecords { get; set; } = new List<WaterIntakeRecord>();

    [InverseProperty("User")]
    public virtual ICollection<WeightRecord> WeightRecords { get; set; } = new List<WeightRecord>();

    [InverseProperty("User")]
    public virtual ICollection<WorkoutPlan> WorkoutPlans { get; set; } = new List<WorkoutPlan>();
    [InverseProperty("User")]
    public virtual Account? Account { get; set; }
}
