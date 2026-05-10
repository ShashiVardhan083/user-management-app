using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserManagementApp.Data;
using UserManagementApp.Models.ViewModels;

namespace UserManagementApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext context;

        public AccountController(AppDbContext context)
        {
            this.context = context;
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = context.Users.FirstOrDefault(u =>
                u.Username == model.Username &&
                u.PasswordHash == model.Password &&
                u.IsActive);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Credentials are wrong");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username ?? "User name"),
                new Claim(ClaimTypes.Role, user.Role ?? "User Role")
            };

            var identity = new ClaimsIdentity(claims, "Cookies");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("Cookies", principal);

            return RedirectToAction("Index", "Profile");
        }



        public IActionResult AccessDenied() => View();

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("Login");
        }
    }
}
