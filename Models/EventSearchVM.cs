using System;
using System.Collections.Generic;

namespace SmartEventWeb.Models
{
    public class EventSearchVM
    {
        public int? CategoryId { get; set; }
        public int? VenueId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal? MaxPrice { get; set; }

        public List<Category> Categories { get; set; } = new();
        public List<Venue> Venues { get; set; } = new();
        public List<Event> Results { get; set; } = new();
    }
}
