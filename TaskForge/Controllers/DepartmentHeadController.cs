using Microsoft.AspNetCore.Mvc;

namespace TaskForge.Controllers
{
    public class DepartmentHeadController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
