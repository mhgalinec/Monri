using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Monri.API.Models;
using Monri.API.Services;
using Monri.Core.Models;
using Monri.Core.Models.DTOs;
using Monri.Core.Services;
using Monri.Data.Repositories;
using Xunit;

namespace Monri.Tests
{
    public class ApiUserServiceTests
    {
        [Fact]
        public async Task InsertUser_ReturnsFailure_WhenCanInsertUserFails()
        {
            var repoMock = new Mock<IUserRepository>();
            var httpProviderMock = new Mock<IHttpClientProviderService>();
            var emailMock = new Mock<IEmailService>();
            var loggerMock = new Mock<ILogger<UserService>>();
            var appSettings = Options.Create(new AppSettings { JSONPlaceholder = "https://json" });

            var maxEntriesError = Error.MaxEntriesReached;
            repoMock.Setup(r => r.CanInsertUser(It.IsAny<UserDTO>())).ReturnsAsync(Result.Failure<bool>(maxEntriesError));

            var service = new UserService(repoMock.Object, httpProviderMock.Object, emailMock.Object, loggerMock.Object, appSettings);

            var dto = new UserDTO { Email = "a@b.com" };
            var result = await service.InsertUser(dto);

            Assert.True(result.IsFailure);
            Assert.Equal(maxEntriesError, result.Error);

            repoMock.Verify(r => r.CanInsertUser(dto), Times.Once);
            httpProviderMock.Verify(h => h.GetRequest<List<UserDTO>>(It.IsAny<string>()), Times.Never);
            emailMock.Verify(e => e.SendEmail(It.IsAny<UserDTO>()), Times.Never);
        }

        [Fact]
        public async Task InsertUser_UsesApiDetails_AndInsertsWithDetails_WhenApiReturnsData()
        {
            var repoMock = new Mock<IUserRepository>();
            var httpProviderMock = new Mock<IHttpClientProviderService>();
            var emailMock = new Mock<IEmailService>();
            var loggerMock = new Mock<ILogger<UserService>>();
            var appSettings = Options.Create(new AppSettings { JSONPlaceholder = "https://json" });

            repoMock.Setup(r => r.CanInsertUser(It.IsAny<UserDTO>())).ReturnsAsync(Result.Success(true));
            var apiUser = new UserDTO { Email = "a@b.com", Name = "ApiName", Username = "user1" };
            httpProviderMock.Setup(h => h.GetRequest<List<UserDTO>>(It.IsAny<string>()))
                .ReturnsAsync(Result.Success(new List<UserDTO> { apiUser }));

            repoMock.Setup(r => r.InsertUserWithDetails(It.IsAny<UserDTO>())).ReturnsAsync(Result.Success(99));
            emailMock.Setup(e => e.SendEmail(It.IsAny<UserDTO>())).ReturnsAsync(Result.Success(true));

            var service = new UserService(repoMock.Object, httpProviderMock.Object, emailMock.Object, loggerMock.Object, appSettings);

            var formDto = new UserDTO { Email = "a@b.com", FirstName = "F", LastName = "L" };
            var result = await service.InsertUser(formDto);

            Assert.True(result.IsSuccess);
            Assert.Equal(99, result.Value);

            repoMock.Verify(r => r.InsertUserWithDetails(It.Is<UserDTO>(u => u.Email == formDto.Email && u.FirstName == formDto.FirstName && u.LastName == formDto.LastName)), Times.Once);
            emailMock.Verify(e => e.SendEmail(formDto), Times.Once);
        }

        [Fact]
        public async Task InsertUser_InsertsFormOnly_WhenApiReturnsEmpty()
        {
            var repoMock = new Mock<IUserRepository>();
            var httpProviderMock = new Mock<IHttpClientProviderService>();
            var emailMock = new Mock<IEmailService>();
            var loggerMock = new Mock<ILogger<UserService>>();
            var appSettings = Options.Create(new AppSettings { JSONPlaceholder = "https://json" });

            repoMock.Setup(r => r.CanInsertUser(It.IsAny<UserDTO>())).ReturnsAsync(Result.Success(true));
            httpProviderMock.Setup(h => h.GetRequest<List<UserDTO>>(It.IsAny<string>()))
                .ReturnsAsync(Result.Success(new List<UserDTO>()));

            repoMock.Setup(r => r.InsertUser(It.IsAny<UserDTO>())).ReturnsAsync(Result.Success(55));
            emailMock.Setup(e => e.SendEmail(It.IsAny<UserDTO>())).ReturnsAsync(Result.Success(true));

            var service = new UserService(repoMock.Object, httpProviderMock.Object, emailMock.Object, loggerMock.Object, appSettings);

            var formDto = new UserDTO { Email = "b@c.com", FirstName = "A" };
            var result = await service.InsertUser(formDto);

            Assert.True(result.IsSuccess);
            Assert.Equal(55, result.Value);

            repoMock.Verify(r => r.InsertUser(formDto), Times.Once);
            emailMock.Verify(e => e.SendEmail(formDto), Times.Once);
        }
    }
}