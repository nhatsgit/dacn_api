using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace dacn_api.Models;

[Index("UserId", "Date", Name = "IX_MealRecords_UserId_Date")]
public partial class MealRecord
{
    [Key]
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public DateOnly Date { get; set; }

    [StringLength(20)]
    public string MealType { get; set; } = null!;

    public double? TotalCalories { get; set; }

    [StringLength(200)]
    public string? Note { get; set; }

    [InverseProperty("MealRecord")]
    public virtual ICollection<MealItem> MealItems { get; set; } = new List<MealItem>();

    [ForeignKey("UserId")]
    [InverseProperty("MealRecords")]
    public virtual User User { get; set; } = null!;
}
