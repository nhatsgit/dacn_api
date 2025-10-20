using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace dacn_api.Models;

[Index("UserId", "Date", Name = "IX_ActivityRecords_UserId_Date")]
public partial class ActivityRecord
{
    [Key]
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public DateOnly Date { get; set; }

    [StringLength(50)]
    public string ActivityType { get; set; } = null!;

    public int? Duration { get; set; }

    public double? CaloriesOut { get; set; }

    public int? Steps { get; set; }

    public double? Distance { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("ActivityRecords")]
    public virtual User User { get; set; } = null!;
}
