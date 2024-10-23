using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskForge.Models;
using TaskForge.Service;
using System.Collections.Generic;
using System.Diagnostics;
using TaskForge.DBContext;
using Microsoft.EntityFrameworkCore;

namespace TaskForge.Controllers
{
    public class ManagerController : Controller
    {
        private readonly TaskForgeContext _context;
        private readonly EmployeeService _employeeService;
        private readonly ProjectService _projectService;
        private readonly TaskService _taskService;

        public ManagerController(EmployeeService employeeService, ProjectService projectService, TaskService taskService)
        {
            _employeeService = employeeService;
            _projectService = projectService;
            _taskService = taskService;
        }

        // Hiển thị thông tin cá nhân của Manager
        public IActionResult Index()
        {
            var accountId = User.FindFirstValue("AccountId");
            if (accountId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var manager = _employeeService.GetEmployeeByAccountId(accountId);

            if (manager == null)
            {
                return NotFound("Manager not found.");
            }

            return View(manager);
        }

        // Hiển thị danh sách các dự án mà Manager đang quản lý
        public IActionResult ProjectManage()
        {
            var accountId = User.FindFirstValue("AccountId");
            if (accountId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var ongoingProjects = _projectService.GetProjectsByStatusAndAccount("In Progress", accountId);
            var completedProjects = _projectService.GetProjectsByStatusAndAccount("Completed", accountId);
            var cancelledProjects = _projectService.GetProjectsByStatusAndAccount("Cancelled", accountId);
            var departments = _projectService.GetAllDepartments();

            ViewBag.OngoingProjects = ongoingProjects;
            ViewBag.CompletedProjects = completedProjects;
            ViewBag.CancelledProjects = cancelledProjects;
            ViewBag.Departments = departments;

            return View();
        }

        // Hiển thị chi tiết dự án
        public IActionResult ProjectDetails(int id)
        {
            var project = _projectService.GetProjectById(id);
            if (project == null)
            {
                return NotFound();
            }

            // Lấy các task thuộc dự án
            var tasks = _taskService.GetTasksByProjectId(id);
            ViewBag.Tasks = tasks;

            // Lọc các phòng ban liên quan đến dự án hiện tại
            ViewBag.Departments = project.Departments;
            return View(project);
        }



        [HttpPost]
        public IActionResult CreateProject(string ProjectName, string Description, DateTime Deadline, List<string> SelectedDepartments)
        {
            var accountId = User.FindFirstValue("AccountId"); // Lấy AccountId của người dùng hiện tại
            if (accountId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                // Tự động sinh ra ProjectId là số nguyên
                int projectId = GenerateProjectId();

                // Tạo đối tượng dự án mới với ProjectId
                var project = new Project
                {
                    ProjectId = projectId,  // Gán ProjectId kiểu số nguyên
                    ProjectName = ProjectName,
                    Description = Description,
                    Deadline = Deadline,
                    Status = "In Progress"
                };

                _projectService.AddProject(project);

                // Thêm phòng ban vào dự án
                foreach (var deptId in SelectedDepartments)
                {
                    var department = _projectService.GetDepartmentById(deptId);
                    if (department != null)
                    {
                        project.Departments.Add(department);
                    }
                }

                _projectService.UpdateProjectDepartments(project, SelectedDepartments);

                _projectService.AddEmployeeToProject(accountId, project.ProjectId, "Manager");

                return RedirectToAction("ProjectManage");
            }

            ViewBag.Departments = _projectService.GetAllDepartments();
            return View("ProjectManage");
        }

        // Phương thức sinh mã ProjectId ngẫu nhiên đơn giản
        private int GenerateProjectId()
        {
            var random = new Random();
            return random.Next(10000, 99999); // Sinh số ngẫu nhiên trong khoảng 10000 đến 99999
        }



        // Xử lý việc tạo Task cho dự án
        [HttpPost]
        public IActionResult CreateTaskForProject(int projectId, string taskName, string description, DateTime deadline, List<string> departmentIds, int priority, DateTime assignmentDate)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Tạo đối tượng Task mới
                    var task = new TaskForge.Models.Task
                    {
                        TaskName = taskName,
                        Description = description,
                        Deadline = deadline,
                        AssignmentDate = assignmentDate,
                        Priority = priority, // Lưu ưu tiên dưới dạng số
                        ProjectId = projectId,
                        Status = "In Progress"
                    };

                    // Gọi service để tạo task và gán phòng ban
                    _taskService.CreateTask(task, departmentIds);

                    return RedirectToAction("ProjectDetails", new { id = projectId });
                }
                catch (Exception ex)
                {
                    // Bắt lỗi và hiển thị lỗi ra giao diện
                    ViewBag.ErrorMessage = $"Error creating task: {ex.Message}";
                }
            }

            // Trường hợp ModelState không hợp lệ hoặc có lỗi xảy ra
            var project = _projectService.GetProjectById(projectId);
            ViewBag.Project = project;
            ViewBag.Departments = _projectService.GetAllDepartments(); // Hiển thị lại danh sách phòng ban
            ViewBag.Tasks = _taskService.GetTasksByProjectId(projectId);

            return View("ProjectDetails");
        }

        // Sửa dự án
        [HttpPost]
        public IActionResult EditProject(int ProjectId, string ProjectName, string Description, DateTime Deadline, List<string> SelectedDepartments)
        {
            var project = _projectService.GetProjectById(ProjectId);
            if (project == null)
            {
                return NotFound("Project not found.");
            }

            // Cập nhật thông tin dự án
            project.ProjectName = ProjectName;
            project.Description = Description;
            project.Deadline = Deadline;

            // Gọi service để cập nhật dự án và danh sách phòng ban
            _projectService.UpdateProject(project, SelectedDepartments);

            return RedirectToAction("ProjectManage"); // Tải lại trang quản lý dự án
        }

        public IActionResult DeleteProject(int projectId)
        {
            try
            {
                // Gọi service để xóa dự án và xử lý tất cả liên kết liên quan
                _projectService.DeleteProject(projectId);

                // Trả về kết quả thành công, chuyển hướng về trang quản lý dự án
                return RedirectToAction("ProjectManage");
            }
            catch (Exception ex)
            {
                // Nếu có lỗi xảy ra, trả về trang lỗi hoặc hiển thị thông báo lỗi ra giao diện
                ViewBag.ErrorMessage = $"Error deleting project: {ex.Message}";
                return View("Error");
            }
        }

        [HttpPost]
        public IActionResult EditTask(string TaskId, string TaskName, string Description, int Priority, DateTime AssignmentDate, DateTime Deadline)
        {
            var task = _taskService.GetTaskById(TaskId);
            if (task == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin task
            task.TaskName = TaskName;
            task.Description = Description;
            task.Priority = Priority;
            task.AssignmentDate = AssignmentDate;
            task.Deadline = Deadline;

            _taskService.UpdateTask(task);

            return RedirectToAction("ProjectDetails", new { id = task.ProjectId });
        }

        [HttpPost]
        public IActionResult DeleteTask(string TaskId)
        {
            var task = _taskService.GetTaskById(TaskId);  // Lấy task từ DB
            if (task == null)
            {
                return NotFound();
            }

            // Lưu ProjectId của task trước khi xóa để chuyển hướng sau khi xóa
            var projectId = task.ProjectId;

            _taskService.DeleteTask(TaskId);  // Xóa task

            // Chuyển hướng về trang chi tiết của dự án sau khi xóa task
            return RedirectToAction("ProjectDetails", new { id = projectId });
        }

    }
}
