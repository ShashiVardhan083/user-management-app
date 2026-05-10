using Microsoft.AspNetCore.Mvc;

namespace UserManagementApp.Controllers
{
    public class MyNameController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}