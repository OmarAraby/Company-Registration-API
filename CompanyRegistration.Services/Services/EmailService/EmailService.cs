using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace CompanyRegistration.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<bool> SendOtpEmailAsync(string toEmail, string otpCode, string companyName)
        {
            //throw new NotImplementedException();
            try
            {
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
                var fromEmail = _configuration["EmailSettings:FromEmail"];
                var fromPassword = _configuration["EmailSettings:FromPassword"];

                using var client = new SmtpClient(smtpServer, smtpPort)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromEmail, fromPassword)
                };
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail!, "Company Sign-Up System"),
                    Subject = "Email Verification - OTP Code",
                    Body = $@"
                        <h2>Welcome {companyName}!</h2>
                        <p>Thank you for registering with our system.</p>
                        <p>Your OTP verification code is: <strong>{otpCode}</strong></p>
                        <p>This code will expire in 15 minutes.</p>
                        <p>If you didn't request this registration, please ignore this email.</p>
                    ",
                    IsBodyHtml = true
                };
                mailMessage.To.Add(toEmail);
                await client.SendMailAsync(mailMessage);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        }
    }

