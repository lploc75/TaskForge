using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskForge.Service;
using TaskForge.Models;
using System.Collections.Generic;

namespace TaskForge.Controllers
{
    [Authorize(Roles = "Staff")]
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

            return View(employee);
        }

        public IActionResult Task()
        {
            string accountId = User.FindFirst("AccountId")?.Value;

            if (string.IsNullOrEmpty(accountId))
            {
                return RedirectToAction("Error", "Home");
            }

            List<Subtask> assignedSubtasks = _employeeService.GetAssignedSubtasks(accountId) ?? new List<Subtask>();
            return View(assignedSubtasks);  // Truyền Model trực tiếp
        }
        public IActionResult Setting()
        {
            return View();
        }
    }
}
