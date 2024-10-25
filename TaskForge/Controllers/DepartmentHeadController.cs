using Microsoft.AspNetCore.Mvc;
using TaskForge.Models;
using TaskForge.Service;
using System.Security.Claims;
using System.Collections.Generic;
using TaskForge.Repository;

namespace TaskForge.Controllers
{
    public class DepartmentHeadController : Controller
    {
        private readonly TaskService _taskService;
        private readonly SubtaskService _subtaskService;
        private readonly TeamService _teamService;
        private readonly EmployeeRepository _employeeRepository;

        public DepartmentHeadController(TaskService taskService, SubtaskService subtaskService, TeamService teamService, EmployeeRepository employeeRepository)
        {
            _taskService = taskService;
            _subtaskService = subtaskService;
            _teamService = teamService;
            _employeeRepository = employeeRepository;
        }

        public IActionResult Index()
        {
            var accountId = User.FindFirstValue("AccountId");
            if (accountId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var employee = _employeeRepository.GetEmployeeByAccountId(accountId);
            if (employee == null)
            {
                return NotFound("Employee not found.");
            }

            var tasks = _taskService.GetTasksByDepartment(employee.DeptId);
            ViewBag.Tasks = tasks;

            return View();
        }

        public IActionResult TaskDetail(string taskId)
        {
            var task = _taskService.GetTaskById(taskId);
            if (task == null) return NotFound("Task not found.");

            var subtasks = _subtaskService.GetSubtasksByTaskId(taskId);
            ViewBag.Task = task;
            ViewBag.Subtasks = subtasks;

            var accountId = User.FindFirstValue("AccountId");
            var employee = _employeeRepository.GetEmployeeByAccountId(accountId);
            if (employee == null) return NotFound("Employee not found.");

            var teams = _teamService.GetTeamsByDepartment(employee.DeptId);
            ViewBag.Teams = teams;

            return View();
        }

        [HttpPost]
        public IActionResult CreateSubtask(Subtask subtask)
        {
            // Thiết lập giá trị mặc định
            subtask.Status = "In Progress"; // Trạng thái mặc định
            subtask.Priority ??= 1;         // Ưu tiên mặc định (nếu chưa có giá trị)
            subtask.Difficulty ??= 1;       // Độ khó mặc định (nếu chưa có giá trị)

            _subtaskService.CreateSubtask(subtask);
            return RedirectToAction("TaskDetail", new { taskId = subtask.TaskId });
        }


        [HttpPost]
        public IActionResult EditSubtask(Subtask subtask)
        {
            _subtaskService.UpdateSubtask(subtask);
            return RedirectToAction("TaskDetail", new { taskId = subtask.TaskId });
        }


        [HttpPost]
        public IActionResult DeleteSubtask(string subtaskId, string taskId)
        {
            _subtaskService.DeleteSubtask(subtaskId);
            return RedirectToAction("TaskDetail", new { taskId = taskId });
        }

    }
}
