using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace dacn_api.Models;

[Index("UserId", "Date", Name = "IX_WeightRecords_UserId_Date")]
public partial class WeightRecord
{
    [Key]
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public DateOnly Date { get; set; }

    public double Weight { get; set; }

    [Column("BMI")]
    public double? Bmi { get; set; }

    public double? IdealWeight { get; set; }

    [StringLength(200)]
    public string? Note { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("WeightRecords")]
    public virtual User User { get; set; } = null!;
}
