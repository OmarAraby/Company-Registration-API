

using System.ComponentModel.DataAnnotations;

namespace CompanyRegistration.Data
{
    public class VerificationToken
    {
        public int Id { get; set; }

        public int CompanyId { get; set; }

        [Required]
        [MaxLength(6)]
        public string OtpCode { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime ExpiresAt { get; set; }

        public bool IsUsed { get; set; } = false;

        public VerificationTokenType TokenType { get; set; }

        // Navigation property
        public virtual Company Company { get; set; } = null!;
    }
}
