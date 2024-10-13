using Microsoft.AspNetCore.Mvc;

namespace TaskForge.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
