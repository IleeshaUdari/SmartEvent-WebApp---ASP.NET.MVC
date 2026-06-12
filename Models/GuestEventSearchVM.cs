using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartEventWeb.Models
{
    public class GuestEventSearchVM
    {
        // Filters (optional)
        public string? Keyword { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        public string? VenueKeyword { get; set; }

        // Results (restricted columns only)
        public List<GuestEventRowVM> Results { get; set; } = new();
    }

    public class GuestEventRowVM
    {
        public int EventId { get; set; }
        public string Title { get; set; } = "";
        public DateTime EventDate { get; set; }
        public string VenueName { get; set; } = "";
        public bool IsFull { get; set; }  // Available/Full flag
    }
}
