using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace dacn_api.Models;

public partial class SleepRecord
{
    [Key]
    public int Id { get; set; }

    public Guid UserId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime StartTime { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime EndTime { get; set; }

    public int? DurationMinutes { get; set; }

    [StringLength(50)]
    public string? SleepQuality { get; set; }

    [StringLength(50)]
    public string? SleepType { get; set; }

    [StringLength(200)]
    public string? Notes { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("SleepRecords")]
    public virtual User User { get; set; } = null!;
}
