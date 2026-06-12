using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SmartEventWeb.Models
{
    public class Review
    {
        public int ReviewId { get; set; }

        [Required]
        public int EventId { get; set; }
        public Event? Event { get; set; }

        // ✅ Required (NOT NULL in DB)
        [Required]
        public string UserId { get; set; } = "";
        public IdentityUser? User { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        public string Comment { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
