using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskForge.Service;
using TaskForge.Models;
using System.Collections.Generic;
using TaskForge.Repository;

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
            // Lấy 'AccountId' từ các claims của người dùng hiện tại đã đăng nhập
            string accountId = User.FindFirst("AccountId")?.Value;

            //// Nếu 'AccountId' là null hoặc rỗng, tức là người dùng chưa xác thực
            //if (string.IsNullOrEmpty(accountId))
            //{
            //    // Chuyển hướng đến action "Error" trong controller "Home" nếu không tìm thấy tài khoản hợp lệ
            //    return RedirectToAction("Error", "Home");
            //}

            // Lấy danh sách các subtasks được gán cho tài khoản dựa vào accountId,
            // nếu không tìm thấy subtasks nào, khởi tạo danh sách trống
            List<Subtask> assignedSubtasks = _employeeService.GetAssignedSubtasks(accountId) ?? new List<Subtask>();

            // Trả về View và truyền danh sách subtasks đã gán vào Model
            return View(assignedSubtasks);
        }

        [HttpPost]
        public IActionResult UpdateProfile(string accountId, Employee updatedEmployee)
        {
            if (string.IsNullOrEmpty(accountId))
            {
                return RedirectToAction("Error", "Home");
            }

            if (ModelState.IsValid)
            {
                // Cập nhật thông tin người dùng dựa trên AccountId
                bool result = _employeeService.UpdateEmployeeProfile(accountId, updatedEmployee);

                ViewBag.Message = result ? "Profile updated successfully." : "Failed to update profile.";
            }
            else
            {
                // Ghi log chi tiết lỗi trong ModelState
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Error in field {state.Key}: {error.ErrorMessage}");
                    }
                }
                ViewBag.Message = "Invalid input. Please check the form and try again.";
            }

            // Lấy lại dữ liệu mới nhất từ database để truyền vào view Setting
            var employee = _employeeService.GetEmployeeByAccountId(accountId);
            return View("Setting", employee);
        }
        public IActionResult Setting()
        {
            string accountId = User.FindFirst("AccountId")?.Value;

            if (string.IsNullOrEmpty(accountId))
            {
                return RedirectToAction("Error", "Home");
            }

            Employee employee = _employeeService.GetEmployeeByAccountId(accountId);

            if (employee == null || employee.Account == null)
            {
                return RedirectToAction("Error", "Home");
            }

            return View(employee);
        }

        [HttpPost]
        public IActionResult UpdateStatus(string subtaskId, string status)
        {
            // Gọi service để cập nhật trạng thái task
            var result = _employeeService.UpdateSubtaskStatus(subtaskId, status);
            if (result)
            {
                return RedirectToAction("Task");
            }
            return RedirectToAction("Error", "Home");
        }

    }
}
