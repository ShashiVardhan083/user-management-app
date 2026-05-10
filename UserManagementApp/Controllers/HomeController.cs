using Microsoft.AspNetCore.Mvc;

namespace UserManagementApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
    }
}
