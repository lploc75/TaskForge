using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskForge.Models;
using TaskForge.Service;
using System.Collections.Generic;

namespace TaskForge.Controllers
{
    public class ManagerController : Controller
    {
        private readonly EmployeeService _employeeService;
        private readonly ProjectService _projectService;

        public ManagerController(EmployeeService employeeService, ProjectService projectService)
        {
            _employeeService = employeeService;
            _projectService = projectService;
        }

        // Hiển thị thông tin cá nhân của Manager
        public IActionResult Index()
        {
            // Lấy AccountId từ Claims
            var accountId = User.FindFirstValue("AccountId");
            if (accountId == null)
            {
                return RedirectToAction("Login", "Account"); // Chuyển hướng đến trang đăng nhập nếu chưa đăng nhập
            }

            // Lấy thông tin nhân viên là Manager thông qua EmployeeService
            Employee manager = _employeeService.GetEmployeeByAccountId(accountId);

            if (manager == null)
            {
                return NotFound("Manager not found.");
            }

            // Truyền thông tin Manager vào View
            return View(manager); // Hiển thị thông tin trong view Index.cshtml
        }

        //// Hiển thị danh sách dự án mà Manager đang quản lý
        //public IActionResult ProjectManage()
        //{
        //    // Lấy AccountId từ Claims
        //    var accountId = User.FindFirstValue("AccountId");
        //    if (accountId == null)
        //    {
        //        return RedirectToAction("Login", "Account"); // Chuyển hướng đến trang đăng nhập nếu chưa đăng nhập
        //    }

        //    // Lấy thông tin nhân viên là Manager thông qua EmployeeService
        //    Employee manager = _employeeService.GetEmployeeByAccountId(accountId);

        //    if (manager == null)
        //    {
        //        return NotFound("Manager not found.");
        //    }

        //    // Lấy danh sách các dự án mà Manager phụ trách từ ProjectService
        //    List<Project> ongoingProjects = _projectService.GetProjectsByStatusAndManager("Đang tiến hành", accountId);
        //    List<Project> completedProjects = _projectService.GetProjectsByStatusAndManager("Hoàn thành", accountId);
        //    List<Project> cancelledProjects = _projectService.GetProjectsByStatusAndManager("Đã hủy", accountId);

        //    // Truyền dữ liệu qua ViewData để hiển thị trong View mà không cần ViewModel
        //    ViewData["OngoingProjects"] = ongoingProjects;
        //    ViewData["CompletedProjects"] = completedProjects;
        //    ViewData["CancelledProjects"] = cancelledProjects;

        //    return View(); // Hiển thị dữ liệu trong ProjectManage.cshtml
        //}

        // Xử lý logic tạo dự án mới
        [HttpPost]
        public IActionResult CreateProject(Project newProject)
        {
            if (ModelState.IsValid)
            {
                _projectService.AddNewProject(newProject);
                return RedirectToAction("ProjectManage"); // Quay lại trang quản lý dự án sau khi tạo
            }

            // Nếu không hợp lệ, giữ lại thông tin đã nhập và hiển thị thông báo lỗi
            return View("ProjectManage", newProject);
        }
    }
}
