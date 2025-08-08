

using Microsoft.AspNetCore.Http;

namespace CompanyRegistration.Services
{
    public record FileUploadRequest(IFormFile File);
}
