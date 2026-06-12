using System;
using System.ComponentModel.DataAnnotations;

namespace SmartEventWeb.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        [Required]
        public int EventId { get; set; }
        public Event? Event { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string SeatType { get; set; } = "Standard"; // Standard / VIP

        [Range(1, 1000)]
        public int Quantity { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
