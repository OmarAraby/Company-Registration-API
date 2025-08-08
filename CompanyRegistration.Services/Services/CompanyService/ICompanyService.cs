

namespace CompanyRegistration.Services
{
    public interface ICompanyService
    {
        Task<APIResult<CompanyResponseDto>> SignUpAsync(CompanySignUpRequestDto request);
        Task<APIResult<string>> VerifyOtpAsync(VerifyOtpRequestDto request);
        Task<APIResult<string>> SetPasswordAsync(SetPasswordRequestDto request);
        Task<APIResult<CompanyResponseDto>> LoginAsync(LoginRequestDto request);
        Task<APIResult<CompanyResponseDto>> GetCompanyByIdAsync(int id);
    }
}
