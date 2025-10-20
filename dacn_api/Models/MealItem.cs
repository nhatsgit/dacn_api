using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace dacn_api.Models;

public partial class MealItem
{
    [Key]
    public int Id { get; set; }

    public int MealRecordId { get; set; }

    public int? FoodId { get; set; }

    public double Quantity { get; set; }

    [StringLength(20)]
    public string? Unit { get; set; }

    public double? Calories { get; set; }

    public double? Protein { get; set; }

    public double? Carbs { get; set; }

    public double? Fat { get; set; }

    [ForeignKey("FoodId")]
    [InverseProperty("MealItems")]
    public virtual FoodDatabase? Food { get; set; }

    [ForeignKey("MealRecordId")]
    [InverseProperty("MealItems")]
    public virtual MealRecord MealRecord { get; set; } = null!;
}
