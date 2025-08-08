
using CompanyRegistration.Data;

namespace CompanyRegistration.Repository
{
    public interface IVerificationTokenRepository
    {
        Task<VerificationToken?> GetValidTokenAsync(int companyId, string otpCode, VerificationTokenType tokenType);
        Task<VerificationToken> CreateAsync(VerificationToken token);
        Task<VerificationToken> UpdateAsync(VerificationToken token);
        Task<bool> InvalidateTokensAsync(int comoanyId, VerificationTokenType tokenType);

    }
}
