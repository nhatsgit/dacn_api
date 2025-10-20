using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace dacn_api.Models;

[Table("FoodDatabase")]
public partial class FoodDatabase
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(50)]
    public string? Barcode { get; set; }

    public double Calories { get; set; }

    public double? Protein { get; set; }

    public double? Carbs { get; set; }

    public double? Fat { get; set; }

    [StringLength(50)]
    public string? ServingSize { get; set; }

    [StringLength(50)]
    public string? Type { get; set; }

    public string? Instructions { get; set; }

    [InverseProperty("Food")]
    public virtual ICollection<MealItem> MealItems { get; set; } = new List<MealItem>();
}
