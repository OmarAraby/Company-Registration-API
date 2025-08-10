# 📑 Company Registration API

## 🌟 Overview

The **Company Registration API** is a RESTful ASP.NET Core 7 application that handles:

- Company registration
    
- OTP email verification
    
- Password setup
    
- Authentication
    
- Retrieving company details
    

It uses **Clean Architecture** principles:

- **Entity Framework Core** for persistence
    
- **FluentValidation** for request validation
    
- **Swagger/OpenAPI** for documentation
    

---

## 🗂 Project Structure
```
CompanyRegistration.sln
│
├── CompanyRegistration.API                # API layer (Presentation)
│   ├── Controllers
│   │   └── CompanyController.cs           # Company endpoints
│   ├── Upload                              # File upload storage
│   ├── appsettings.json                    # App configuration
│   ├── CompanyRegistration.API.http        # API test file (VS Code / Rider)
│   └── Program.cs                          # Application entry point
│
├── CompanyRegistration.Data                # Data access & EF Core models
│   ├── Enums
│   │   └── VerificationTokenType.cs
│   ├── Migrations                          # EF Core migrations
│   ├── Models
│   │   ├── Company.cs
│   │   └── VerificationToken.cs
│   └── ApplicationDbContext.cs             # EF Core DbContext
│
├── CompanyRegistration.Repository          # Repository layer
│   ├── Repositories
│   │   ├── CompanyRepository
│   │   │   ├── CompanyRepository.cs
│   │   │   └── ICompanyRepository.cs
│   │   └── VerificationTokenRepository
│   │       ├── VerificationTokenRepository.cs
│   │       └── IVerificationTokenRepository.cs
│
└── CompanyRegistration.Services            # Business logic layer
    ├── DTOS
    │   ├── CompanyResponseDto.cs
    │   ├── CompanySignUpRequestDto.cs
    │   ├── LoginRequestDto.cs
    │   ├── SetPasswordRequestDto.cs
    │   └── VerifyOtpRequestDto.cs
    │
    ├── Services
    │   ├── CompanyService
    │   │   ├── CompanyService.cs
    │   │   └── ICompanyService.cs
    │   └── EmailService
    │       ├── EmailService.cs
    │       └── IEmailService.cs
    │
    ├── Utils
    │   ├── Error
    │   │   ├── APIError.cs
    │   │   └── APIResult.cs
    │   └── HandleFiles
    │       ├── FileService.cs
    │       ├── FileUploadRequest.cs
    │       └── FileUploadResult.cs
    │
    ├── Validators
    │   ├── CompanySignUpRequestDtoValidator.cs
    │   ├── LoginRequestDtoValidator.cs
    │   ├── SetPasswordRequestDtoValidator.cs
    │   └── VerifyOtpRequestDtoValidator.cs
    │
    └── ServiceExtension.cs                  # DI service registrations

```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)

- [PostgreSQL](https://www.postgresql.org/)
    
- [Visual Studio / VS Code](https://code.visualstudio.com/)
    

### Setup

```bash
# Clone repo
git clone https://github.com/yourusername/CompanyRegistration.git
cd CompanyRegistration

# Restore packages
dotnet restore

# Configure database in appsettings.json
"DefaultConnection": "Host=localhost;Port=5432;Database=CompanyDb;Username=postgres;Password=yourpassword;"

# Apply migrations
dotnet ef migrations add InitialCreate
dotnet ef database update

# Run app
dotnet run
```

- API: `http://localhost:5001`
    
- Swagger UI: `http://localhost:5001/swagger`
    

---

## 📌 Key Endpoints

|Method|Endpoint|Description|
|---|---|---|
|POST|`/api/companies/signup`|Register a new company & send OTP|
|POST|`/api/companies/verify-otp`|Verify email OTP|
|POST|`/api/companies/set-password`|Set account password|
|POST|`/api/companies/login`|Authenticate company|
|GET|`/api/companies/{id}`|Get company details by ID|

---

## 🛡 Security

- **Password hashing** with SHA-256 + salt
    
- **OTP verification** for email validation
    
- **Strict validation** via FluentValidation
    
- **File upload safety** – images only (`.jpg`, `.jpeg`, `.png`), max 15MB
    
