using CompanyRegistration.Repository;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace CompanyRegistration.Services
{
    public class CompanySignUpRequestDtoValidator : AbstractValidator<CompanySignUpRequestDto>
    {
        private readonly ICompanyRepository _companyRepository;
        public CompanySignUpRequestDtoValidator(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;

            RuleFor(x => x.CompanyArabicName)
                .NotEmpty().WithMessage("Arabic company name is required")
                .MaximumLength(255).WithMessage("Arabic company name must not exceed 255 characters");

            RuleFor(x => x.CompanyEnglishName)
                .NotEmpty().WithMessage("English company name is required")
                .MaximumLength(255).WithMessage("English company name must not exceed 255 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(255).WithMessage("Email must not exceed 255 characters")
                .MustAsync(BeUniqueEmail).WithMessage("Email address is already registered");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            RuleFor(x => x.WebsiteUrl)
                .Must(BeValidUrl).WithMessage("Invalid website URL format")
                .MaximumLength(500).WithMessage("Website URL must not exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.WebsiteUrl));

            RuleFor(x => x.Logo)
                .Must(BeValidImageFile).WithMessage("Logo must be a valid image file (jpg, jpeg, png)")
                .Must(BeValidFileSize).WithMessage("Logo file size must not exceed 15MB")
                .When(x => x.Logo != null);
        }

        private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
        {
            return !await _companyRepository.EmailExistsAsync(email);
        }

        private bool BeValidUrl(string? url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }

        private bool BeValidImageFile(IFormFile? file)
        {
            if (file == null) return true;
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return allowedExtensions.Contains(extension);
        }

        private bool BeValidFileSize(IFormFile? file)
        {
            if (file == null) return true;
            const int maxSize = 15 * 1024 * 1024; // 15MB
            return file.Length <= maxSize;
        }
    }
}
