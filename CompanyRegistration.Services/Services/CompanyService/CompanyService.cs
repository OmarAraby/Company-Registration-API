

using CompanyRegistration.Data;
using CompanyRegistration.Repository;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace CompanyRegistration.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IVerificationTokenRepository _verificationTokenRepository;
        private readonly IEmailService _emailService;
        private readonly IFileService _fileService;
        private readonly IConfiguration _configuration;

        // inject validators
        private readonly IValidator<CompanySignUpRequestDto> _signUpValidator;
        private readonly IValidator<VerifyOtpRequestDto> _verifyOtpValidator;
        private readonly IValidator<SetPasswordRequestDto> _setPasswordValidator;
        private readonly IValidator<LoginRequestDto> _loginValidator;

        public CompanyService(
            ICompanyRepository companyRepository,
            IVerificationTokenRepository verificationTokenRepository,
            IEmailService emailService,
            IFileService fileService,
            IConfiguration configuration,
            IValidator<CompanySignUpRequestDto> signUpValidator,
            IValidator<VerifyOtpRequestDto> verifyOtpValidator,
            IValidator<SetPasswordRequestDto> setPasswordValidator,
            IValidator<LoginRequestDto> loginValidator
            )
        {
            _companyRepository = companyRepository;
            _verificationTokenRepository = verificationTokenRepository;
            _emailService = emailService;
            _fileService = fileService;
            _configuration = configuration;
            _signUpValidator = signUpValidator;
            _verifyOtpValidator = verifyOtpValidator;
            _setPasswordValidator = setPasswordValidator;
            _loginValidator = loginValidator;
        }

        public async Task<APIResult<CompanyResponseDto>> SignUpAsync(CompanySignUpRequestDto requestDto)
        {
            // Validate request DTO
            var validationResult = await _signUpValidator.ValidateAsync(requestDto);
            if (!validationResult.IsValid)
            {
                return new APIResult<CompanyResponseDto>
                {
                    Success = false,
                    Errors = validationResult.Errors.Select(e => new APIError
                    {
                        Code = "VALIDATION_ERROR",
                        Message = e.ErrorMessage
                    }).ToArray()
                };
            }
            try
            {
                // Handle logo upload
                string? logoPath = null;
                if (requestDto.Logo != null)
                {
                    try
                    {
                        var fileResult = await _fileService.UploadFileAsync(requestDto.Logo);
                        logoPath = fileResult.FileUrl;
                    }
                    catch (ArgumentException ex)
                    {
                        return new APIResult<CompanyResponseDto>
                        {
                            Success = false,
                            Errors = new[] { new APIError { Code = "FILE_ERROR", Message = ex.Message } }
                        };
                    }
                }

                // chck if email already exist
                var existingCompany = await _companyRepository.GetByEmailAsync(requestDto.Email);
                if (existingCompany != null)
                {
                    return new APIResult<CompanyResponseDto>
                    {
                        Success = false,
                        Errors = new[] { new APIError { Code = "EMAIL_EXISTS", Message = "Email is already registered" } }
                    };
                }
                var company = new Company
                {
                    CompanyArabicName = requestDto.CompanyArabicName,
                    CompanyEnglishName = requestDto.CompanyEnglishName,
                    Email = requestDto.Email,
                    PhoneNumber = requestDto.PhoneNumber,
                    WebsiteUrl = requestDto.WebsiteUrl,
                    LogoPath = logoPath,
                    IsEmailVerified = false
                };

                var createdCompany = await _companyRepository.CreateAsync(company);

                // generate otp

                var otpCode = GenerateOtpCode();
                var VerficationToken = new VerificationToken
                {
                    CompanyId = createdCompany.CompanyId,
                    OtpCode = otpCode,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                    TokenType = VerificationTokenType.EmailVerification
                };
                await _verificationTokenRepository.CreateAsync(VerficationToken);


                // send mail 
                var emailSent = await _emailService.SendOtpEmailAsync(requestDto.Email, otpCode, requestDto.CompanyEnglishName);
                if (!emailSent)
                {
                    return new APIResult<CompanyResponseDto>
                    {
                        Success = false,
                        Errors = new[] { new APIError { Code = "EMAIL_ERROR", Message = "Failed to send verification email" } }
                    };
                }

                var response = MapToCompanyResponseDto(createdCompany);
                return new APIResult<CompanyResponseDto>
                {
                    Success = true,
                    Data = response
                };

            }
            catch (Exception ex)
            {
                return new APIResult<CompanyResponseDto>
                {
                    Success = false,
                    Errors = new[] { new APIError { Code = "SIGNUP_ERROR", Message = "An error occurred during registration" }
                    
                   }
                };
            }
        }

        public async Task<APIResult<string>> VerifyOtpAsync(VerifyOtpRequestDto requestDto)
        {
            // validate dto request 
            var validationResult = await _verifyOtpValidator.ValidateAsync(requestDto);
            if (!validationResult.IsValid)
            {
                return new APIResult<string>
                {
                    Success = false,
                    Errors = validationResult.Errors.Select(e => new APIError
                    {
                        Code = "VALIDATION_ERROR",
                        Message = e.ErrorMessage
                    }).ToArray(),
                };
            
            }
            try
            {
                var token = await _verificationTokenRepository.GetValidTokenAsync(
                    requestDto.CompanyId,
                    requestDto.OtpCode,
                    VerificationTokenType.EmailVerification
                );

                if (token == null)
                {
                    return new APIResult<string>
                    {
                        Success = false,
                        Errors = new[] { new APIError { Code = "INVALID_OTP", Message = "Invalid or expired OTP code" } }
                    };
                }

                // Mark token as used
                //token.IsUsed = true;
                //await _verificationTokenRepository.UpdateAsync(token);

                // Mark company email as verified
                var company = await _companyRepository.GetByIdAsync(requestDto.CompanyId);
                if (company != null)
                {
                    company.IsEmailVerified = true;
                    await _companyRepository.UpdateAsync(company);
                }

                return new APIResult<string>
                {
                    Success = true,
                    Data = "OTP_VERIFIED"
                };
            }
            catch (Exception)
            {
                return new APIResult<string>
                {
                    Success = false,
                    Errors = new[] { new APIError { Code = "VERIFICATION_ERROR", Message = "An error occurred during OTP verification" } }
                };
            }
        }

        public async Task<APIResult<string>> SetPasswordAsync(SetPasswordRequestDto requestDto)
        {
            // validte dto request
            var validationResult = await _setPasswordValidator.ValidateAsync(requestDto);
            if (!validationResult.IsValid)
            {
                return new APIResult<string>
                {
                    Success = false,
                    Errors = validationResult.Errors.Select(e => new APIError
                    {
                        Code = "VALIDATION_ERROR",
                        Message = e.ErrorMessage
                    }).ToArray()
                };
            }
            try
            {
                // Verify OTP is still valid 
                var token = await _verificationTokenRepository.GetValidTokenAsync(
                    requestDto.CompanyId,
                    requestDto.OtpCode,
                    VerificationTokenType.EmailVerification
                );

                if (token == null || token.IsUsed)
                {
                    return new APIResult<string>
                    {
                        Success = false,
                        Errors = new[] { new APIError { Code = "INVALID_SESSION", Message = "Invalid or expired session. Please request a new OTP." } }
                    };
                }

                var company = await _companyRepository.GetByIdAsync(requestDto.CompanyId);
                if (company == null)
                {
                    return new APIResult<string>
                    {
                        Success = false,
                        Errors = new[] { new APIError { Code = "COMPANY_NOT_FOUND", Message = "Company not found" } }
                    };
                }

                // Hash password and save
                company.PasswordHash = HashPassword(requestDto.NewPassword);
                await _companyRepository.UpdateAsync(company);

                // Mark the token as used now
                token.IsUsed = true;
                await _verificationTokenRepository.UpdateAsync(token);

                // Invalidate all verification tokens
                await _verificationTokenRepository.InvalidateTokensAsync(requestDto.CompanyId, VerificationTokenType.EmailVerification);

                return new APIResult<string>
                {
                    Success = true,
                    Data = "PASSWORD_SET"
                };
            }
            catch (Exception)
            {
                return new APIResult<string>
                {
                    Success = false,
                    Errors = new[] { new APIError { Code = "PASSWORD_ERROR", Message = "An error occurred while setting password" } }
                };
            }
        }
        public async Task<APIResult<CompanyResponseDto>> LoginAsync(LoginRequestDto requestDto)
        {
            // validate dto request
            var validationResult = await _loginValidator.ValidateAsync(requestDto);
            if (!validationResult.IsValid)
            {
                return new APIResult<CompanyResponseDto>
                {
                    Success = false,
                    Errors = validationResult.Errors.Select(e => new APIError
                    {
                        Code = "VALIDATION_ERROR",
                        Message = e.ErrorMessage
                    }).ToArray()
                };
            }
            try
            {
                var company = await _companyRepository.GetByEmailAsync(requestDto.Email);
                if (company == null || !company.IsEmailVerified || string.IsNullOrEmpty(company.PasswordHash))
                {
                    return new APIResult<CompanyResponseDto>
                    {
                        Success = false,
                        Errors = new[] { new APIError { Code = "INVALID_CREDENTIALS", Message = "Invalid email or password" } }
                    };
                }

                if (!VerifyPassword(requestDto.Password, company.PasswordHash))
                {
                    return new APIResult<CompanyResponseDto>
                    {
                        Success = false,
                        Errors = new[] { new APIError { Code = "INVALID_CREDENTIALS", Message = "Invalid email or password" } }
                    };
                }

                var response = MapToCompanyResponseDto(company);
                return new APIResult<CompanyResponseDto>
                {
                    Success = true,
                    Data = response
                };
            }
            catch (Exception)
            {
                return new APIResult<CompanyResponseDto>
                {
                    Success = false,
                    Errors = new[] { new APIError { Code = "LOGIN_ERROR", Message = "An error occurred during login" } }
                };
            }
        }

        public async Task<APIResult<CompanyResponseDto>> GetCompanyByIdAsync(int id)
        {
            try
            {
                var company = await _companyRepository.GetByIdAsync(id);
                if (company == null)
                {
                    return new APIResult<CompanyResponseDto>
                    {
                        Success = false,
                        Errors = new[] { new APIError { Code = "COMPANY_NOT_FOUND", Message = "Company not found" } }
                    };
                }

                var response = MapToCompanyResponseDto(company);
                return new APIResult<CompanyResponseDto>
                {
                    Success = true,
                    Data = response
                };
            }
            catch (Exception)
            {
                return new APIResult<CompanyResponseDto>
                {
                    Success = false,
                    Errors = new[] { new APIError { Code = "RETRIEVAL_ERROR", Message = "An error occurred while retrieving company" } }
                };
            }
        }

        // Helper methods
        private string GenerateOtpCode()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[4];
            rng.GetBytes(bytes);
            var randomNumber = Math.Abs(BitConverter.ToInt32(bytes, 0));
            return (randomNumber % 900000 + 100000).ToString(); // Ensures 6 digits
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var saltKey = _configuration["SaltKey"];
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + saltKey));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return HashPassword(password) == hashedPassword;
        }
        private CompanyResponseDto MapToCompanyResponseDto(Company company)
        {
            return new CompanyResponseDto
            {
                Id = company.CompanyId,
                CompanyArabicName = company.CompanyArabicName,
                CompanyEnglishName = company.CompanyEnglishName,
                Email = company.Email,
                PhoneNumber = company.PhoneNumber,
                WebsiteUrl = company.WebsiteUrl,
                LogoPath = company.LogoPath,
                IsEmailVerified = company.IsEmailVerified,
                CreatedAt = company.CreatedAt
            };
        }

    }
}
