using System.ComponentModel.DataAnnotations;

namespace SmartEventWeb.Models.ViewModels
{
    public class InquiryTrackVM : IValidatableObject
    {
        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? Mobile { get; set; }

        [StringLength(12)]
        public string? ReferenceCode { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Email) &&
                string.IsNullOrWhiteSpace(Mobile) &&
                string.IsNullOrWhiteSpace(ReferenceCode))
            {
                yield return new ValidationResult(
                    "Enter Email OR Mobile OR Reference Code to track inquiries.",
                    new[] { nameof(Email), nameof(Mobile), nameof(ReferenceCode) }
                );
            }
        }
    }
}
