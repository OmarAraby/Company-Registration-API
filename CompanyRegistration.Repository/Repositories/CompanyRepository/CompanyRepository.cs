
using CompanyRegistration.Data;
using Microsoft.EntityFrameworkCore;

namespace CompanyRegistration.Repository
{
    public class CompanyRepository: ICompanyRepository
    {
        private readonly ApplicationDbContext _context;
        public CompanyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Company?> GetByIdAsync(int id)
        {
            return await _context.Companies
                .Include(c => c.VerificationTokens)
                .FirstOrDefaultAsync(c => c.CompanyId == id);
        }
        public async Task<Company?> GetByEmailAsync(string email)
        {
            return await _context.Companies
                .Include(c => c.VerificationTokens)
                .FirstOrDefaultAsync(c => c.Email.ToLower() == email.ToLower());
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Companies
                .AnyAsync(c=>c.Email.ToLower()== email.ToLower());
        }

        public async Task<Company> CreateAsync(Company company)
        {
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();
            return company;
        }

        public async Task<Company> UpdateAsync(Company company)
        {
            company.UpdatedAt = DateTime.UtcNow;
            _context.Companies.Update(company);
            await _context.SaveChangesAsync();
            return company;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company == null) return false;

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Company>> GetAllAsync()
        {
            return await _context.Companies
                .Include(c => c.VerificationTokens)
                .ToListAsync();
        }



    }
}
