using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagementApp.Data;
using UserManagementApp.Models.ViewModels;

namespace UserManagementApp.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly AppDbContext context;

        public ProfileController(AppDbContext context)
        {
            this.context = context;
        }

        [Authorize]
        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                var users = context.Users
                    .Where(u => u.IsActive)
                    .ToList();

                return View("AdminIndex", users);
            }

            var username = User.Identity?.Name;

            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var user = context.Users
                .FirstOrDefault(u => u.Username == username);

            if (user == null)
                return NotFound();


            var vm = new UserProfileViewModel
            {
                Username = user.Username,
                Email = user.Email,
                Status = user.IsActive ? "Active" : "Inactive"
            };
            return View(vm);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            var loggedInUsername = User.Identity?.Name;

            UserManagementApp.Models.Entities.User? user;

            if (User.IsInRole("Admin"))
            {
                // Admin editing someone
                if (id == null)
                    return BadRequest();

                user = context.Users.FirstOrDefault(u => u.Id == id);
            }
            else
            {
                // Normal user editing himself
                user = context.Users.FirstOrDefault(u => u.Username == loggedInUsername);
            }

            if (user == null)
                return NotFound();

            // Admin cannot edit other admins
            if (User.IsInRole("Admin") &&
                user.Role == "Admin" &&
                user.Username != loggedInUsername)
                return Forbid();

            // User cannot edit anyone else
            if (!User.IsInRole("Admin") &&
                user.Username != loggedInUsername)
                return Forbid();

            return View(new EditProfileViewModel
            {
                Username = user.Username,
                Email = user.Email
            });
        }

        [Authorize]
        [HttpPost]
        public IActionResult Edit(int? id, EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var loggedInUsername = User.Identity?.Name;

            UserManagementApp.Models.Entities.User? user;

            if (User.IsInRole("Admin"))
            {
                if (id == null)
                    return BadRequest();

                user = context.Users.FirstOrDefault(u => u.Id == id);
            }
            else
            {
                user = context.Users.FirstOrDefault(u => u.Username == loggedInUsername);
            }

            if (user == null)
                return NotFound();

            if (User.IsInRole("Admin") &&
                user.Role == "Admin" &&
                user.Username != loggedInUsername)
                return Forbid();

            if (!User.IsInRole("Admin") &&
                user.Username != loggedInUsername)
                return Forbid();

            user.Username = model.Username;
            user.Email = model.Email;

            context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
