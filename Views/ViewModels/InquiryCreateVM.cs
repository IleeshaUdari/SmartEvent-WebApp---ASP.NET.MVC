using System.ComponentModel.DataAnnotations;

namespace SmartEventWeb.ViewModels
{
    public class InquiryCreateVM
    {
        [Display(Name = "Full Name")]
        [StringLength(80)]
        public string? FullName { get; set; }

        [EmailAddress]
        [StringLength(120)]
        public string? Email { get; set; }

        [Display(Name = "Mobile")]
        [StringLength(20)]
        public string? Mobile { get; set; }

        [Required]
        [StringLength(120)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = string.Empty;
    }
}
