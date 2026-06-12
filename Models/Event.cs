using System;
using System.ComponentModel.DataAnnotations;

namespace SmartEventWeb.Models
{
    public class Event
    {
        public int EventId { get; set; }

        [Required, StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public DateTime EventDate { get; set; }

        [Required, Range(0, 10000000)]
        public decimal Price { get; set; }

        // Ticket logic
        [Range(1, 100000)]
        public int TicketCapacity { get; set; } = 500;

        [Range(0, 100000)]
        public int TicketsSold { get; set; } = 0;

        public int RemainingTickets => TicketCapacity - TicketsSold;

        // Relationships
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public int VenueId { get; set; }
        public Venue? Venue { get; set; }
    }
}
