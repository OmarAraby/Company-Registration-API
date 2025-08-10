using CompanyRegistration.Data;
using CompanyRegistration.Repository;
using CompanyRegistration.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
/// Database configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repository dependencies
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IVerificationTokenRepository, VerificationTokenRepository>();

// service layer dependencies
builder.Services.AddServiceExtension();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

}

app.UseHttpsRedirection();

app.UseAuthorization();

#region HandleFiles

// handle image upload
//if folder doesn't exist create it
var imageFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Upload");
Directory.CreateDirectory(imageFolderPath);


app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(imageFolderPath),
    RequestPath = "/api/static-files"
});
#endregion

app.MapControllers();

app.Run();
