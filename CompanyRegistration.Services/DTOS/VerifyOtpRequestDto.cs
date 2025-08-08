

namespace CompanyRegistration.Services
{
    public class VerifyOtpRequestDto
    {
        public int CompanyId { get; set; }
        public string OtpCode { get; set; } = string.Empty;
    }
}
