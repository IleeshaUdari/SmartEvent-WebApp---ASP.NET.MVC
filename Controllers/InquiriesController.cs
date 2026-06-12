using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEventWeb.Areas.Identity.Data;
using SmartEventWeb.Models;
using SmartEventWeb.ViewModels;

namespace SmartEventWeb.Controllers
{
    public class InquiriesController : Controller
    {
        private readonly SmartEventWebContext _context;

        public InquiriesController(SmartEventWebContext context)
        {
            _context = context;
        }

        // GET: /Inquiries/Create
        public IActionResult Create()
        {
            return View(new InquiryCreateVM());
        }

        // POST: /Inquiries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InquiryCreateVM vm)
        {
            // ✅ Business rule: at least one contact method
            bool hasEmail = !string.IsNullOrWhiteSpace(vm.Email);
            bool hasMobile = !string.IsNullOrWhiteSpace(vm.Mobile);

            if (!hasEmail && !hasMobile)
            {
                ModelState.AddModelError(string.Empty, "Provide at least Email or Mobile number.");
            }

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var inquiry = new Inquiry
            {
                FullName = vm.FullName,
                Email = vm.Email,
                Mobile = vm.Mobile,
                Subject = vm.Subject,
                Message = vm.Message,
                Status = "Submitted",
                CreatedAt = DateTime.UtcNow,
                IsReplied = false,
                ReferenceCode = GenerateRef()
            };

            _context.Inquiries.Add(inquiry);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Confirmation), new { refCode = inquiry.ReferenceCode });
        }

        // GET: /Inquiries/Confirmation?refCode=INQ-XXXXX
        public async Task<IActionResult> Confirmation(string refCode)
        {
            if (string.IsNullOrWhiteSpace(refCode)) return NotFound();

            var inquiry = await _context.Inquiries
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.ReferenceCode == refCode);

            if (inquiry == null) return NotFound();

            return View(inquiry);
        }

        // GET: /Inquiries/Track
        public IActionResult Track()
        {
            return View();
        }

        // POST: /Inquiries/Track
        [HttpPost]
        public async Task<IActionResult> Track(string? referenceCode, string? email, string? mobile)
        {
            referenceCode = referenceCode?.Trim();
            email = email?.Trim();
            mobile = mobile?.Trim();

            if (string.IsNullOrWhiteSpace(referenceCode) &&
                string.IsNullOrWhiteSpace(email) &&
                string.IsNullOrWhiteSpace(mobile))
            {
                ModelState.AddModelError(string.Empty, "Enter Reference Code, Email, or Mobile to search.");
                return View();
            }

            var query = _context.Inquiries.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(referenceCode))
                query = query.Where(i => i.ReferenceCode == referenceCode);

            if (!string.IsNullOrWhiteSpace(email))
                query = query.Where(i => i.Email == email);

            if (!string.IsNullOrWhiteSpace(mobile))
                query = query.Where(i => i.Mobile == mobile);

            var results = await query
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            ViewBag.SearchInput = referenceCode ?? email ?? mobile;
            return View("TrackResults", results);
        }

        private static string GenerateRef()
        {
            // e.g., INQ-A1B2C
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var r = new Random();
            var code = new string(Enumerable.Range(0, 5).Select(_ => chars[r.Next(chars.Length)]).ToArray());
            return $"INQ-{code}";
        }
    }
}
