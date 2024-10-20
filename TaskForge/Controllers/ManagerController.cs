using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskForge.Models;
using TaskForge.Service;

namespace TaskForge.Controllers
{
    public class ManagerController : Controller
    {
        private readonly EmployeeService _employeeService;

        public ManagerController(EmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // Fetch manager information using the logged-in user's AccountId from claims
        public IActionResult Index()
        {
            // Extract AccountId from claims
            var accountId = User.FindFirstValue("AccountId");
            if (accountId == null)
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if not authenticated
            }

            // Retrieve manager information using EmployeeService
            Employee manager = _employeeService.GetEmployeeByAccountId(accountId);

            // Check if the manager was found
            if (manager == null)
            {
                return NotFound("Manager not found.");
            }

            // Pass the manager model to the view
            return View(manager);
        }
    }
}
