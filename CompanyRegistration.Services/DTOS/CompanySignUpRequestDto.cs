

using Microsoft.AspNetCore.Http;

namespace CompanyRegistration.Services
{
    public class CompanySignUpRequestDto
    {
        public string CompanyArabicName { get; set; } = string.Empty;
        public string CompanyEnglishName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? WebsiteUrl { get; set; }
        public IFormFile? Logo { get; set; }
    }
}
