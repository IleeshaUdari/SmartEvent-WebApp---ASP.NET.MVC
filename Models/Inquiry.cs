using System.ComponentModel.DataAnnotations;

namespace SmartEventWeb.Models
{
    public class Inquiry
    {
        public int Id { get; set; }

        [StringLength(20)]
        public string ReferenceCode { get; set; } = string.Empty;

        [StringLength(80)]
        public string? FullName { get; set; }

        [StringLength(120)]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? Mobile { get; set; }

        [Required, StringLength(120)]
        public string Subject { get; set; } = string.Empty;

        [Required, StringLength(1000)]
        public string Message { get; set; } = string.Empty;

        [StringLength(20)]
        public string Status { get; set; } = "Submitted"; // Submitted / Replied

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsReplied { get; set; } = false;

        public DateTime? RepliedDate { get; set; }
    }
}
