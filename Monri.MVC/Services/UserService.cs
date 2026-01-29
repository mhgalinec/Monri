using Microsoft.Extensions.Options;
using Monri.Core.Models;
using Monri.Core.Models.DTOs;
using Monri.Core.Services;
using Monri.MVC.Models;

namespace Monri.MVC.Services
{
    public interface IUserService
    {
        Task<Result<int>> InsertUser(UserDTO userDTO);
    }

    public class UserService : IUserService
    {
        private readonly IHttpClientProviderService _httpClientProviderService;
        private readonly AppSettings _appSettings;
        private readonly ILogger<UserService> _logger;
        public UserService(IHttpClientProviderService httpClientProviderService, IOptions<AppSettings> appSettings, ILogger<UserService> logger)
        {
            _httpClientProviderService = httpClientProviderService;
            _appSettings = appSettings.Value;
            _logger = logger;
        }

        public async Task<Result<int>> InsertUser(UserDTO userDTO)
        {
            try
            {
                var endpoint = $"{_appSettings.BaseUrl}{_appSettings.InsertUser}";
                _logger.LogError("Trying to do a post request to " + endpoint);
                return await _httpClientProviderService.PostRequest<int, UserDTO>(endpoint, userDTO);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                return Result.Failure<int>(new Error(212,e.Message));
            }
        }
    }
}
