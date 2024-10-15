using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskForge.Service;
using TaskForge.Models;

namespace TaskForge.Controllers
{
    [Authorize]
    public class StaffController : Controller
    {
        private readonly EmployeeService _employeeService;

        public StaffController(EmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public IActionResult Index()
        {
            string accountId = User.FindFirst("AccountId")?.Value;

            if (string.IsNullOrEmpty(accountId))
            {
                return RedirectToAction("Error", "Home");
            }

            Employee employee = _employeeService.GetEmployeeByAccountId(accountId);

            if (employee == null)
            {
                return RedirectToAction("Error", "Home");
            }

            // Pass the employee, including the Account information, to the view
            return View(employee);
        }
    }
}
