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
            StaffAndLeader kpiData = _employeeService.GetKPIData(accountId);

            if (employee == null || kpiData == null)
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

            var taskDifficultyStats = _employeeService.GetTaskDifficultyStats(accountId);

            ViewData["SimpleTasks"] = taskDifficultyStats.ContainsKey(1) ? taskDifficultyStats[1] : 0;
            ViewData["ModerateTasks"] = taskDifficultyStats.ContainsKey(2) ? taskDifficultyStats[2] : 0;
            ViewData["ComplexTasks"] = taskDifficultyStats.ContainsKey(3) ? taskDifficultyStats[3] : 0;
            ViewData["ExtremelyComplexTasks"] = taskDifficultyStats.ContainsKey(4) ? taskDifficultyStats[4] : 0;

            ViewData["TotalKPI"] = kpiData.TotalKpi * 20;
            ViewData["TotalTimeliness"] = kpiData.TotalTimeliness * 20;
            ViewData["TotalTeamwork"] = kpiData.TotalTeamwork * 20;

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
