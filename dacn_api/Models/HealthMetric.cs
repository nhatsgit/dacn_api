using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace dacn_api.Models;

[Index("UserId", "Date", Name = "IX_HealthMetrics_UserId_Date")]
public partial class HealthMetric
{
    [Key]
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public DateOnly Date { get; set; }

    public int? HeartRate { get; set; }

    public int? BloodPressureSys { get; set; }

    public int? BloodPressureDia { get; set; }

    public double? Glucose { get; set; }

    public int? SleepDuration { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("HealthMetrics")]
    public virtual User User { get; set; } = null!;
}
