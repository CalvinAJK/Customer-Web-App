using Auth0.AspNetCore.Authentication;
using Customer_Web_App.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace Customer_Web_App.Controllers
{
    public class AccountController : Controller
    {
        public async Task Login(string returnUrl = "/")
        {
            var authenticationProperties = new
                LoginAuthenticationPropertiesBuilder()
                    .WithRedirectUri(returnUrl)
                    .Build();

            await HttpContext.ChallengeAsync(
                Auth0Constants.AuthenticationScheme, authenticationProperties);
        }

        [Authorize]
        public async Task Logout()
        {
            var authenticationProperties = new
                LogoutAuthenticationPropertiesBuilder()
                    .WithRedirectUri(Url.Action("Index", "Home"))
                    .Build();

            await HttpContext.SignOutAsync(
                Auth0Constants.AuthenticationScheme, authenticationProperties);

            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
        }

        [Authorize]
        public IActionResult Profile()
        {
            // Get user claims
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            // Retrieve app metadata claims
            var addressValue = User.Claims.FirstOrDefault(c => c.Type == "app_metadata." + userId + ".address")?.Value;
            var phoneNumberValue = User.Claims.FirstOrDefault(c => c.Type == "app_metadata." + userId + ".phoneNumber")?.Value;

            // Populate UserProfileViewModel
            var userProfile = new UserProfileViewModel()
                {
                    Name = User.Identity.Name,
                    EmailAddress = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                    ProfileImage = User.Claims.FirstOrDefault(c => c.Type == "picture")?.Value,
                    Address = addressValue ?? "Default Address",  // Default to "Default Address" if not present
                    PhoneNumber = phoneNumberValue ?? "123456"  // Default to 0 if not present
            };

            return View(userProfile);
        }



        [Authorize]
        public IActionResult Claims()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
