using System.ComponentModel.DataAnnotations;

namespace SmartEventWeb.Models;

public class BookingCreateVM
{
    public int EventId { get; set; }

    [Required]
    public string SeatType { get; set; } = "Standard";

    [Range(1, 999)]
    public int Quantity { get; set; } = 1;
}
