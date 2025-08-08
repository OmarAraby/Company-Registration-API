using CompanyRegistration.Data;

namespace CompanyRegistration.Repository
{
    public interface ICompanyRepository
    {
        Task<Company> GetByIdAsync(int companyId);
        Task<Company> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task<Company> CreateAsync(Company company);
        Task<Company> UpdateAsync(Company company);
        Task<bool> DeleteAsync(int companyId);
        Task<IEnumerable<Company>> GetAllAsync();

    }
}
