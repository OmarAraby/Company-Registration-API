


namespace CompanyRegistration.Services
{
    public interface IEmailService
    {
        Task<bool> SendOtpEmailAsync(string toEmail, string otpCode, string companyName);

    }
}
