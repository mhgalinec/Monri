using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Monri.API.Models;
using Monri.API.Services;
using Monri.Core.Models;
using Monri.Core.Models.DTOs;
using Xunit;

namespace Monri.Tests
{
    public class EmailServiceTests
    {
        [Fact]
        public async Task SendEmail_ReturnsFalse_WhenSmtpFails()
        {
            var emailSettings = new EmailSettings
            {
                SmtpHost = "invalid.host.example", 
                SmtpPort = 25,
                Username = "u",
                Password = "p",
                From = "from@test.local"
            };

            var appSettings = Options.Create(new AppSettings { EmailSettings = emailSettings });
            var loggerMock = new Mock<ILogger<UserService>>();

            var service = new EmailService(appSettings, loggerMock.Object);

            var user = new UserDTO { Email = "noone@example.test", FirstName = "X", LastName = "Y" };

            var result = await service.SendEmail(user);

            Assert.True(result.IsFailure);
        }
    }
}