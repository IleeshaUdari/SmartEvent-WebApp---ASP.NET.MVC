using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEventWeb.Areas.Identity.Data;

namespace SmartEventWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly SmartEventWebContext _context;

        public HomeController(SmartEventWebContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var featured = _context.Events
                .Include(e => e.Category)
                .Include(e => e.Venue)
                .OrderBy(e => e.EventDate)
                .Take(5)
                .ToList();

            return View(featured);
        }

        public IActionResult Privacy() => View();
    }
}
