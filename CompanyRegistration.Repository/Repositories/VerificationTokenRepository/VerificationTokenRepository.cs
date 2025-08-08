

using CompanyRegistration.Data;
using Microsoft.EntityFrameworkCore;

namespace CompanyRegistration.Repository
{
    public class VerificationTokenRepository : IVerificationTokenRepository
    {
        private readonly ApplicationDbContext _context;

        public VerificationTokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<VerificationToken?> GetValidTokenAsync(int companyId, string otpCode, VerificationTokenType tokenType)
        {
            return await _context.VerificationTokens
                .FirstOrDefaultAsync(t =>
                    t.CompanyId == companyId &&
                    t.OtpCode == otpCode &&
                    t.TokenType == tokenType &&
                    !t.IsUsed &&
                    t.ExpiresAt > DateTime.UtcNow);
        }

        public async Task<VerificationToken> CreateAsync(VerificationToken token)
        {
            _context.VerificationTokens.Add(token);
            await _context.SaveChangesAsync();
            return token;
        }

        public async Task<VerificationToken> UpdateAsync(VerificationToken token)
        {
            _context.VerificationTokens.Update(token);
            await _context.SaveChangesAsync();
            return token;

        }
        public async Task<bool> InvalidateTokensAsync(int companyId, VerificationTokenType tokenType)
        {
            var tokens = await _context.VerificationTokens
                .Where(t => t.CompanyId == companyId && t.TokenType == tokenType && !t.IsUsed)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.IsUsed = true;
            }

            await _context.SaveChangesAsync();
            return true;
        }


    }
}
