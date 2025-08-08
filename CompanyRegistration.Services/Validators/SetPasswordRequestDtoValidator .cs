using FluentValidation;
using System.Text.RegularExpressions;


namespace CompanyRegistration.Services
{
    public class SetPasswordRequestDtoValidator : AbstractValidator<SetPasswordRequestDto>
    {
        public SetPasswordRequestDtoValidator()
        {
            RuleFor(x => x.CompanyId)
                .GreaterThan(0).WithMessage("Valid company ID is required");

            RuleFor(x => x.OtpCode)
                .NotEmpty().WithMessage("OTP code is required")
                .Length(6).WithMessage("OTP code must be 6 digits")
                .Matches(@"^\d{6}$").WithMessage("OTP code must contain only digits");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(7).WithMessage("Password must be at least 7 characters long")
                .Must(HaveUppercaseLetter).WithMessage("Password must contain at least one uppercase letter")
                .Must(HaveSpecialCharacter).WithMessage("Password must contain at least one special character")
                .Must(HaveNumber).WithMessage("Password must contain at least one number");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Password confirmation is required")
                .Equal(x => x.NewPassword).WithMessage("Passwords do not match");
        }

        private bool HaveUppercaseLetter(string password)
        {
            return Regex.IsMatch(password, @"[A-Z]");
        }

        private bool HaveSpecialCharacter(string password)
        {
            return Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]");
        }

        private bool HaveNumber(string password)
        {
            return Regex.IsMatch(password, @"[0-9]");
        }
    }
}
