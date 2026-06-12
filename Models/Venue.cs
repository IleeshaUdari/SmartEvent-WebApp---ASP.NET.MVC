using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartEventWeb.Models
{
    public class Venue
    {
        public int VenueId { get; set; }

        [Required, StringLength(120)]
        public string Name { get; set; } = string.Empty;

        [StringLength(120)]
        public string Location { get; set; } = string.Empty;

        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}
