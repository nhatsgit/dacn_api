using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace dacn_api.Models;

[Index("UserId", "Date", Name = "IX_WaterIntakeRecords_UserId_Date")]
public partial class WaterIntakeRecord
{
    [Key]
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public DateOnly Date { get; set; }

    public int Amount { get; set; }

    public int? Target { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? Time { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("WaterIntakeRecords")]
    public virtual User User { get; set; } = null!;
}
