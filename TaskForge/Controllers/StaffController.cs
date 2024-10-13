using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TaskForge.Controllers
{
    [Authorize]
    public class StaffController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
