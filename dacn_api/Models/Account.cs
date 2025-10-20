
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace dacn_api.Models
{
    public class Account : IdentityUser
    {
        // Liên kết 1-1 với User
        public Guid? UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
