using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Monri.Core.Models;
using Monri.Core.Models.DTOs;
using Monri.Core.Services;
using Monri.MVC.Models;
using Monri.MVC.Services;
using Xunit;

namespace Monri.Tests
{
    public class UserServiceTests
    {
        [Fact]
        public async Task InsertUser_ReturnsSuccess_WhenProviderReturnsSuccess()
        {
            var appSettings = Options.Create(new AppSettings { BaseUrl = "https://api", InsertUser = "/users" });
            var userDto = new UserDTO { FirstName = "John", LastName = "Doe", Email = "john@doe.com" };
            var expectedEndpoint = "https://api/users";

            var httpProviderMock = new Mock<IHttpClientProviderService>();
            httpProviderMock
                .Setup(s => s.PostRequest<int, UserDTO>(It.Is<string>(e => e == expectedEndpoint), It.IsAny<UserDTO>()))
                .ReturnsAsync(Result.Success(42));

            var loggerMock = new Mock<ILogger<UserService>>();

            var service = new UserService(httpProviderMock.Object, appSettings, loggerMock.Object);

            var result = await service.InsertUser(userDto);

            Assert.True(result.IsSuccess);
            Assert.Equal(42, result.Value);
            httpProviderMock.Verify(s => s.PostRequest<int, UserDTO>(expectedEndpoint, userDto), Times.Once);
        }

        [Fact]
        public async Task InsertUser_ReturnsFailure_WhenProviderReturnsFailure()
        {
            var appSettings = Options.Create(new AppSettings { BaseUrl = "https://api", InsertUser = "/users" });
            var userDto = new UserDTO { FirstName = "Jane" };
            var expectedEndpoint = "https://api/users";

            var error = Error.Create("D00", "Unable to save");

            var httpProviderMock = new Mock<IHttpClientProviderService>();
            httpProviderMock
                .Setup(s => s.PostRequest<int, UserDTO>(It.IsAny<string>(), It.IsAny<UserDTO>()))
                .ReturnsAsync(Result.Failure<int>(error));

            var loggerMock = new Mock<ILogger<UserService>>();

            var service = new UserService(httpProviderMock.Object, appSettings, loggerMock.Object);

            var result = await service.InsertUser(userDto);

            Assert.True(result.IsFailure);
            Assert.Equal(error, result.Error);
            httpProviderMock.Verify(s => s.PostRequest<int, UserDTO>(expectedEndpoint, It.IsAny<UserDTO>()), Times.Once);
        }

        [Fact]
        public async Task InsertUser_LogsAndReturnsFailure_OnException()
        {
            var appSettings = Options.Create(new AppSettings { BaseUrl = "https://api", InsertUser = "/users" });
            var userDto = new UserDTO { FirstName = "E" };

            var httpProviderMock = new Mock<IHttpClientProviderService>();
            httpProviderMock
                .Setup(s => s.PostRequest<int, UserDTO>(It.IsAny<string>(), It.IsAny<UserDTO>()))
                .ThrowsAsync(new InvalidOperationException("boom"));

            var loggerMock = new Mock<ILogger<UserService>>();

            var service = new UserService(httpProviderMock.Object, appSettings, loggerMock.Object);

            var result = await service.InsertUser(userDto);

            Assert.True(result.IsFailure);
            Assert.Equal("boom", result.Error.Message);
        }
    }
}