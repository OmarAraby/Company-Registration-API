

using Microsoft.AspNetCore.Http;
using System.Linq;

namespace CompanyRegistration.Services
{
    public class FileService : IFileService
    {
        private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Upload");
        private readonly List<string> _allowedExtensions = new() { ".jpg", ".jpeg", ".png" };
        private const int _maxFileSize = 15 * 1024 * 1024;
        public async Task<FileUploadResult> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("No file provided or file is empty");
            }

            if (file.Length > _maxFileSize)
                throw new ArgumentException("File is too large");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
                throw new ArgumentException("File must be a jpg, jpeg, png");

            var filePath = Path.Combine(_uploadPath, $"{Guid.NewGuid()}{extension}");

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return new FileUploadResult($"/api/static-files/{Path.GetFileName(filePath)}");
        }
    }
}
