

using CompanyRegistration.Repository;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CompanyRegistration.Services
{
    public static class ServiceExtension
    {
        public static void AddServiceExtension(this IServiceCollection services)
        {
            // Repository dependencies
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IVerificationTokenRepository, VerificationTokenRepository>();

            // Service dependencies
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IEmailService, EmailService>();

            // file Handler
            services.AddScoped<IFileService, FileService>();



            services.AddValidatorsFromAssembly(typeof(ServiceExtension).Assembly);
        }
    }
}
