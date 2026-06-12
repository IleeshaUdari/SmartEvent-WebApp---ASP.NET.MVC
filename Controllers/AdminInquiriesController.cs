using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEventWeb.Areas.Identity.Data;

namespace SmartEventWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminInquiriesController : Controller
    {
        private readonly SmartEventWebContext _context;

        public AdminInquiriesController(SmartEventWebContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _context.Inquiries
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            ViewBag.Total = list.Count;
            ViewBag.Replied = list.Count(i => i.IsReplied);
            ViewBag.Pending = list.Count(i => !i.IsReplied);

            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> MarkReplied(int id)
        {
            var inquiry = await _context.Inquiries.FindAsync(id);
            if (inquiry == null) return NotFound();

            inquiry.IsReplied = true;
            inquiry.RepliedDate = DateTime.UtcNow;
            inquiry.Status = "Replied";

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
