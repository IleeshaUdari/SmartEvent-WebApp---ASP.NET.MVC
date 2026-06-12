using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEventWeb.Areas.Identity.Data;
using SmartEventWeb.Models;
using SmartEventWeb.Models.ViewModels;

namespace SmartEventWeb.Controllers
{
    [Authorize]
    public class BookingsController : Controller
    {
        private readonly SmartEventWebContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public BookingsController(SmartEventWebContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ✅ NEW: Booking page from navbar (hub)
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var upcoming = await _context.Events
                .Include(e => e.Category)
                .Include(e => e.Venue)
                .Where(e => e.EventDate >= DateTime.Today)
                .OrderBy(e => e.EventDate)
                .ToListAsync();

            return View(upcoming);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int eventId)
        {
            var ev = await _context.Events
                .Include(e => e.Category)
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            if (ev == null) return NotFound();

            ViewBag.EventTitle = ev.Title;
            ViewBag.Capacity = ev.TicketCapacity;
            ViewBag.Sold = ev.TicketsSold;
            ViewBag.Remaining = ev.RemainingTickets;

            return View(new BookingCreateVM { EventId = eventId, SeatType = "Standard", Quantity = 1 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingCreateVM vm)
        {
            var ev = await _context.Events.FirstOrDefaultAsync(e => e.EventId == vm.EventId);
            if (ev == null) return NotFound();

            ViewBag.EventTitle = ev.Title;
            ViewBag.Capacity = ev.TicketCapacity;
            ViewBag.Sold = ev.TicketsSold;
            ViewBag.Remaining = ev.RemainingTickets;

            if (!ModelState.IsValid) return View(vm);

            var remaining = ev.RemainingTickets;

            // ✅ must show 495 sold, 5 remaining, request 10 rejected
            if (vm.Quantity > remaining)
            {
                ModelState.AddModelError("",
                    $"Requested {vm.Quantity} tickets is not available. {ev.TicketsSold} sold, {remaining} remaining.");
                return View(vm);
            }

            var userId = _userManager.GetUserId(User)!;

            _context.Bookings.Add(new Booking
            {
                EventId = ev.EventId,
                SeatType = vm.SeatType,
                Quantity = vm.Quantity,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            });

            // ✅ FIX: update TicketsSold (NOT RemainingTickets)
            ev.TicketsSold += vm.Quantity;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Booking confirmed successfully!";
            return RedirectToAction("My");
        }

        // Booking access page in navbar (only after login)
        public async Task<IActionResult> My()
        {
            var userId = _userManager.GetUserId(User)!;

            var bookings = await _context.Bookings
                .Include(b => b.Event)
                .ThenInclude(e => e!.Venue)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return View(bookings);
        }
    }
}
