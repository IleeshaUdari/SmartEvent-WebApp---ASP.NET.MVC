using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEventWeb.Areas.Identity.Data;
using SmartEventWeb.Models;

namespace SmartEventWeb.Controllers
{
    public class EventsController : Controller
    {
        private readonly SmartEventWebContext _context;

        public EventsController(SmartEventWebContext context)
        {
            _context = context;
        }
        // ✅ Guest Restricted Search
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GuestSearch(string? keyword, DateTime? fromDate, DateTime? toDate, string? venueKeyword)
        {
            var q = _context.Events
                .Include(e => e.Venue)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
                q = q.Where(e => e.Title.Contains(keyword));

            if (!string.IsNullOrWhiteSpace(venueKeyword))
                q = q.Where(e => e.Venue.Name.Contains(venueKeyword));

            if (fromDate.HasValue)
                q = q.Where(e => e.EventDate >= fromDate.Value);

            if (toDate.HasValue)
                q = q.Where(e => e.EventDate <= toDate.Value);

            // ✅ restricted projection (ONLY allowed fields)
            var results = await q
                .OrderBy(e => e.EventDate)
                .Select(e => new GuestEventRowVM
                {
                    EventId = e.EventId,
                    Title = e.Title,
                    EventDate = e.EventDate,
                    VenueName = e.Venue.Name + " (" + e.Venue.Location + ")",
                    IsFull = e.RemainingTickets <= 0
                })
                .ToListAsync();

            var vm = new GuestEventSearchVM
            {
                Keyword = keyword,
                FromDate = fromDate,
                ToDate = toDate,
                VenueKeyword = venueKeyword,
                Results = results
            };

            return View(vm);
        }
        public IActionResult Search(EventSearchVM vm)
        {
            vm.Categories = _context.Categories.OrderBy(c => c.Name).ToList();
            vm.Venues = _context.Venues.OrderBy(v => v.Name).ToList();

            var q = _context.Events
                .Include(e => e.Category)
                .Include(e => e.Venue)
                .AsQueryable();

            // ✅ Short LINQ filter block (evidence)
            if (vm.CategoryId.HasValue) q = q.Where(e => e.CategoryId == vm.CategoryId.Value);
            if (vm.VenueId.HasValue) q = q.Where(e => e.VenueId == vm.VenueId.Value);
            if (vm.FromDate.HasValue) q = q.Where(e => e.EventDate >= vm.FromDate.Value);
            if (vm.ToDate.HasValue) q = q.Where(e => e.EventDate <= vm.ToDate.Value);
            if (vm.MaxPrice.HasValue) q = q.Where(e => e.Price <= vm.MaxPrice.Value);

            vm.Results = q.OrderBy(e => e.EventDate).ToList();
            return View(vm);
        }
    }
}
