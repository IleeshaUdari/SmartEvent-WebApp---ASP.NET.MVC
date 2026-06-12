using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEventWeb.Areas.Identity.Data;
using SmartEventWeb.Models;

namespace SmartEventWeb.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly SmartEventWebContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ReviewsController(SmartEventWebContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ✅ Guests + Members can view reviews
        // /Reviews OR /Reviews?eventId=1
        public async Task<IActionResult> Index(int? eventId)
        {
            ViewBag.Events = await _context.Events
                .OrderBy(e => e.EventDate)
                .ToListAsync();

            ViewBag.SelectedEventId = eventId;

            var query = _context.Reviews
                .Include(r => r.Event)
                .OrderByDescending(r => r.CreatedAt)
                .AsQueryable();

            if (eventId.HasValue)
                query = query.Where(r => r.EventId == eventId.Value);

            var reviews = await query.ToListAsync();
            return View(reviews);
        }

        // ✅ Members ONLY can submit reviews
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(int eventId, int rating, string comment)
        {
            // 1) validate event exists
            var ev = await _context.Events.FirstOrDefaultAsync(e => e.EventId == eventId);
            if (ev == null) return NotFound();

            // 2) allow reviews only after event date (professional rule)
            if (ev.EventDate.Date > DateTime.Today)
            {
                TempData["Error"] = "Reviews can be submitted only after the event date.";
                return RedirectToAction(nameof(Index), new { eventId });
            }

            // 3) allow only members who booked that event
            var userId = _userManager.GetUserId(User);

            // ✅ Adjust this if your Booking uses a different field name than UserId
            var hasBooking = await _context.Bookings.AnyAsync(b => b.EventId == eventId && b.UserId == userId);

            if (!hasBooking)
            {
                TempData["Error"] = "Only members with bookings can submit reviews.";
                return RedirectToAction(nameof(Index), new { eventId });
            }

            // 4) save review
            var review = new Review
            {
                EventId = eventId,
                UserId = userId,
                Rating = rating,
                Comment = comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Review submitted successfully.";
            return RedirectToAction(nameof(Index), new { eventId });
        }
    }
}
