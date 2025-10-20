using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace dacn_api.Models;

public partial class Notification
{
    [Key]
    public int Id { get; set; }

    public Guid UserId { get; set; }

    [StringLength(50)]
    public string Type { get; set; } = null!;

    [StringLength(200)]
    public string Message { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime Time { get; set; }

    public bool? IsRead { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Notifications")]
    public virtual User User { get; set; } = null!;
}
