

using System.ComponentModel.DataAnnotations;

namespace CompanyRegistration.Data
{
    public class Company
    {
        public int CompanyId { get; set; }
        [Required]
        [MaxLength(255)]
        public string CompanyArabicName { get; set; }
        [Required]
        [MaxLength(255)]
        public string CompanyEnglishName { get; set; }
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }
        [Phone]
        [MaxLength(15)]
        public string PhoneNumber { get; set; }

        [Url]
        [MaxLength(500)]
        public string? WebsiteUrl { get; set; }
        public string? LogoPath { get; set; }
        public string? PasswordHash { get; set; }

        public bool IsEmailVerified { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }


        // Navigation property
        public virtual ICollection<VerificationToken> VerificationTokens { get; set; } = new List<VerificationToken>();
    }

}

