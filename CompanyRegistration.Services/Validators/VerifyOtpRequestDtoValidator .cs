

using FluentValidation;

namespace CompanyRegistration.Services
{
    public class VerifyOtpRequestDtoValidator : AbstractValidator<VerifyOtpRequestDto>
    {
        public VerifyOtpRequestDtoValidator()
        {
            RuleFor(x => x.CompanyId)
                .GreaterThan(0).WithMessage("Valid company ID is required");

            RuleFor(x => x.OtpCode)
                .NotEmpty().WithMessage("OTP code is required")
                .Length(6).WithMessage("OTP code must be 6 digits")
                .Matches(@"^\d{6}$").WithMessage("OTP code must contain only digits");
        }
    }
}
