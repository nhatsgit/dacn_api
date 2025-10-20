using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace dacn_api.Models;

public partial class MedicationReminder
{
    [Key]
    public int Id { get; set; }

    public Guid UserId { get; set; }

    [StringLength(100)]
    public string MedicineName { get; set; } = null!;

    [StringLength(50)]
    public string? Dosage { get; set; }

    [StringLength(50)]
    public string? Frequency { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public TimeOnly ReminderTime { get; set; }

    [StringLength(200)]
    public string? Note { get; set; }

    public bool? IsActive { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("MedicationReminders")]
    public virtual User User { get; set; } = null!;
}
