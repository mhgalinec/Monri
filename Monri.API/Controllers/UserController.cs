using Microsoft.AspNetCore.Mvc;
using Monri.API.Services;
using Monri.Core.Models.DTOs;

namespace Monri.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : BaseController<UserController>
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> InsertUser([FromBody] UserDTO userDTO)
        {
            var result = await _userService.InsertUser(userDTO);

            if (result.IsFailure)
            {
                _logger.LogError(result.Error.Message);
                return BadResult(result.Error);
            }

            return Ok(result.Value);
        }
    }
}
