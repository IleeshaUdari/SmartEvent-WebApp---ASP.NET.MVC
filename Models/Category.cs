using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartEventWeb.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [Required, StringLength(60)]
        public string Name { get; set; } = string.Empty;

        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}
