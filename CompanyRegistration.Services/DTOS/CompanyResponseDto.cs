using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyRegistration.Services
{
    public class CompanyResponseDto
    {
        public int Id { get; set; }
        public string CompanyArabicName { get; set; } = string.Empty;
        public string CompanyEnglishName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? WebsiteUrl { get; set; }
        public string? LogoPath { get; set; }
        public bool IsEmailVerified { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
