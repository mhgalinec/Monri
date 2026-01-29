using Microsoft.Extensions.Options;
using Monri.API.Models;
using Monri.Core.Models;
using Monri.Core.Models.DTOs;
using Monri.Core.Services;
using Monri.Data.Repositories;

namespace Monri.API.Services
{
    public interface IUserService
    {
        Task<Result<int>> InsertUser(UserDTO userDTO);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpClientProviderService _httpClientProviderService;
        private readonly IEmailService _emailService;
        private readonly ILogger<UserService> _logger;
        private readonly AppSettings _appSettings;

        public UserService(IUserRepository userRepository, IHttpClientProviderService httpClientProviderService,
            IEmailService emailService, ILogger<UserService> logger, IOptions<AppSettings> appSettings)
        {
            _userRepository = userRepository;
            _httpClientProviderService = httpClientProviderService;
            _emailService = emailService;
            _logger = logger;
            _appSettings = appSettings.Value;
        }

        public async Task<Result<int>> InsertUser(UserDTO userDTO)
        {
            _logger.LogInformation($"Form submission started for user: {userDTO.Email}");

            var canInsert = await _userRepository.CanInsertUser(userDTO);
            if (canInsert.IsFailure)
            {
                _logger.LogWarning($"User {userDTO.Email} reached max form submissions per minute");
                return Result.Failure<int>(canInsert.Error);
            }

            _logger.LogInformation($"API call for {userDTO.Email}");
            var userDetailsResult = await _httpClientProviderService.GetRequest<List<UserDTO>>($"{_appSettings.JSONPlaceholder}?email={userDTO.Email}");

            if (userDetailsResult.IsSuccess && userDetailsResult.Value.Any())
            {
                var apiUser = userDetailsResult.Value.FirstOrDefault();
                if (apiUser != null)
                {
                    _logger.LogInformation($"API returned additional info for {userDTO.Email}");

                    apiUser.FirstName = userDTO.FirstName;
                    apiUser.LastName = userDTO.LastName;

                    var detailsInsertResult = await _userRepository.InsertUserWithDetails(apiUser);

                    if (detailsInsertResult.IsSuccess)
                    {
                        _logger.LogInformation($"User {userDTO.Email} was saved to the DB");
                        await _emailService.SendEmail(userDTO);
                    }
                    else
                        _logger.LogError($"Failed to save {userDTO.Email} with additional info");

                    return detailsInsertResult;
                }
            }
            else
            {
                _logger.LogInformation($"API didnt return any additional info for {userDTO.Email}, attempting to save the form data only");

            }
            var userInsertResult = await _userRepository.InsertUser(userDTO);

            if (userInsertResult.IsSuccess)
            {
                _logger.LogInformation($"User {userDTO.Email} with form only details was saved to the DB");
                await _emailService.SendEmail(userDTO);
            }
            else
                _logger.LogError($"Failed to save {userDTO.Email} with form only info");

            return userInsertResult;
        }
    }
}
