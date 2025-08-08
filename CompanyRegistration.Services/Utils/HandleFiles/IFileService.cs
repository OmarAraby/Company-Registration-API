

using Microsoft.AspNetCore.Http;

namespace CompanyRegistration.Services
{
    public interface IFileService
    {
        Task<FileUploadResult> UploadFileAsync(IFormFile file);
    }
}
