using Epood.Models;
using Epood.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace Epood.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController
            (
            UserManager<ApplicationUser> userManager
            )
        {
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            return View(user);
            
        }

        // Settings
        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            var user = await _userManager.GetUserAsync(User);

            var vm = new AccountSettingsViewModel
            {
                Email = user.Email,
                UserName = user.UserName,
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Settings(AccountSettingsViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var user = await _userManager.GetUserAsync(User);

            user.Email = vm.Email;
            user.UserName = vm.UserName;

            await _userManager.UpdateAsync(user);

            if (!string.IsNullOrEmpty(vm.NewPassword))
            {
                var passwordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _userManager.ResetPasswordAsync(user, passwordToken, vm.NewPassword);
            }

            return RedirectToAction("Index");
        }

    }
}
