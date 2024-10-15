using Microsoft.AspNetCore.Mvc;

namespace TaskForge.Controllers
{
    public class LeaderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
