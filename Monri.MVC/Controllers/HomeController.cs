using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Monri.Core.Models.DTOs;
using Monri.MVC.Models;
using Monri.MVC.Services;

namespace Monri.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<HomeController> _logger;
        public HomeController(IUserService userService, ILogger<HomeController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Submit(UserFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"The model state was invalid: FirstName {model.FirstName}, LastName {model.LastName}, Email {model.Email}");
                return View("Index", model);
            }

            _logger.LogInformation($"Form submission received for {model.Email}");

            var userDTO = new UserDTO() { FirstName = model.FirstName, LastName = model.LastName, Email = model.Email };
            var result = await _userService.InsertUser(userDTO);

            if (result.IsSuccess)
            {
                _logger.LogInformation($"User was successfully inserted {model.Email}");
                TempData["SuccessMessage"] = "Data was successfully sent";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = result.Error.Message;
            return View("Index", model);

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
