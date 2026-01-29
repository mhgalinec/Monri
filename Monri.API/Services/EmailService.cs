using Microsoft.Extensions.Options;
using Monri.API.Models;
using Monri.Core.Models;
using Monri.Core.Models.DTOs;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Monri.API.Services
{
    public interface IEmailService
    {
        Task<Result<bool>> SendEmail(UserDTO user);
    }
    public class EmailService : IEmailService
    {
        private readonly AppSettings _appSettings;
        private readonly ILogger<UserService> _logger;
        public EmailService(IOptions<AppSettings> appSettings, ILogger<UserService> logger)
        {
            _appSettings = appSettings.Value;
            _logger = logger;
        }

        public async Task<Result<bool>> SendEmail(UserDTO user)
        {
            var smtpHost = _appSettings.EmailSettings.SmtpHost;
            var smtpPort = _appSettings.EmailSettings.SmtpPort;
            var smtpUser = _appSettings.EmailSettings.Username;
            var smtpPass = _appSettings.EmailSettings.Password;
            var fromEmail = _appSettings.EmailSettings.From;

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };

            var mail = new MailMessage();
            mail.From = new MailAddress(fromEmail);
            mail.To.Add(user?.Email ?? "");
            mail.Subject = "The Form has been submitted";
            mail.IsBodyHtml = true;

            var sb = new StringBuilder();

            sb.AppendLine("<h3>Form Data</h3>");
            sb.AppendLine($"Ime: {user.FirstName}<br/>");
            sb.AppendLine($"Prezime: {user.LastName}<br/>");
            sb.AppendLine($"Email: {user.Email}<br/><br/>");

            sb.AppendLine("<h3>Additional Info</h3>");
            sb.AppendLine($"Name: {user.Name}<br/>");
            sb.AppendLine($"Username: {user.Username}<br/>");
            sb.AppendLine($"Phone: {user.Phone}<br/>");
            sb.AppendLine($"Website: {user.Website}<br/>");

            if (user.Address != null)
            {
                sb.AppendLine($"Address: {user.Address.Street}, {user.Address.Suite}, {user.Address.City}, {user.Address.Zipcode}<br/>");
            }

            if (user.Company != null)
            {
                sb.AppendLine($"Company: {user.Company.Name}, {user.Company.CatchPhrase}, {user.Company.Bs}<br/>");
            }

            mail.Body = sb.ToString();

            _logger.LogInformation($"Sending Email for {user.Email}");

            try
            {
                await client.SendMailAsync(mail);
                _logger.LogInformation($"Email sent for {user.Email}");
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occured while sending an email to {user.Email}");
                return Result.Failure<bool>(Error.Exception);
            }
        }
    }
}
