using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskForge.Service;
using TaskForge.Models;
using System.Collections.Generic;

namespace TaskForge.Controllers
{
    [Authorize(Roles = "staff")]
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

            int assignedTaskCount = _employeeService.GetAssignedSubtasks(accountId).Count;
            ViewData["AssignedTaskCount"] = assignedTaskCount;

            var (beforeDeadline, onDeadline, afterDeadline) = _employeeService.GetCompletedTaskStats(accountId);

            int totalCompleted = beforeDeadline + onDeadline + afterDeadline;

            double beforeDeadlinePercent = totalCompleted > 0 ? (beforeDeadline * 100.0 / totalCompleted) : 0;
            double onDeadlinePercent = totalCompleted > 0 ? (onDeadline * 100.0 / totalCompleted) : 0;
            double afterDeadlinePercent = totalCompleted > 0 ? (afterDeadline * 100.0 / totalCompleted) : 0;

            ViewData["TotalCompleted"] = totalCompleted;
            ViewData["BeforeDeadlinePercent"] = beforeDeadlinePercent;
            ViewData["OnDeadlinePercent"] = onDeadlinePercent;
            ViewData["AfterDeadlinePercent"] = afterDeadlinePercent;

            var (completed, canceled, incomplete) = _employeeService.GetTaskStatusCounts(accountId);
            ViewData["CompletedCount"] = completed;
            ViewData["CanceledCount"] = canceled;
            ViewData["IncompleteCount"] = incomplete;

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
    }
}
