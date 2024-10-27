using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskForge.Models;
using TaskForge.Service;
using System.Collections.Generic;
using System.Diagnostics;
using TaskForge.DBContext;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace TaskForge.Controllers
{
    public class ManagerController : Controller
    {
        private readonly TaskForgeContext _context;
        private readonly EmployeeService _employeeService;
        private readonly ProjectService _projectService;
        private readonly TaskService _taskService;
        private readonly SubtaskService _subtaskService;
        private readonly TeamService _teamService;


        public ManagerController(EmployeeService employeeService, ProjectService projectService, TaskService taskService, SubtaskService subtaskService, TeamService teamService)
        {
            _employeeService = employeeService;
            _projectService = projectService;
            _taskService = taskService;
            _subtaskService = subtaskService;
            _teamService = teamService;
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

        public IActionResult ProjectManage(int? page, int pageSize = 6, string status = null, DateTime? deadline = null, string departmentId = null, string searchTerm = null)
        {
            var accountId = User.FindFirstValue("AccountId");
            if (accountId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Fetch all projects for the manager
            var allProjects = _projectService.GetAllProjectsByManagerAccountId(accountId);

            // Apply filters if provided
            if (!string.IsNullOrEmpty(status))
            {
                allProjects = allProjects.Where(p => p.Status == status).ToList();
            }

            if (deadline.HasValue)
            {
                allProjects = allProjects.Where(p => p.Deadline <= deadline.Value).ToList();
            }

            // Filter by department ID if provided
            if (!string.IsNullOrEmpty(departmentId))
            {
                allProjects = allProjects
                    .Where(p => p.Departments.Any(d => d.DeptId == departmentId))
                    .ToList();
            }

            // Apply search term filter
            if (!string.IsNullOrEmpty(searchTerm))
            {
                allProjects = allProjects.Where(p => p.ProjectName != null && p.ProjectName.Contains(searchTerm)).ToList();
            }

            // Apply pagination
            int pageNumber = page ?? 1;
            ViewBag.AllProjects = allProjects.ToPagedList(pageNumber, pageSize);
            ViewBag.Departments = _projectService.GetAllDepartments();

            // Pass current filter values to the view for persistence
            ViewBag.CurrentStatus = status;
            ViewBag.CurrentDeadline = deadline;
            ViewBag.CurrentDepartmentId = departmentId;
            ViewBag.SearchTerm = searchTerm; // Để giữ lại giá trị tìm kiếm hiện tại

            return View();
        }



        // Hiển thị chi tiết dự án
        public IActionResult ProjectDetails(int id, int? page, string status, DateTime? startDate, DateTime? endDate, int? priority, string department)
        {
            var project = _projectService.GetProjectById(id);
            if (project == null)
            {
                return NotFound();
            }

            // Lấy danh sách task ban đầu
            var tasks = _taskService.GetTasksByProjectId(id);

            // Áp dụng bộ lọc nếu có
            if (!string.IsNullOrEmpty(status))
            {
                tasks = tasks.Where(t => t.Status == status).ToList();
            }
            if (startDate.HasValue)
            {
                tasks = tasks.Where(t => t.AssignmentDate >= startDate.Value).ToList();
            }
            if (endDate.HasValue)
            {
                tasks = tasks.Where(t => t.Deadline <= endDate.Value).ToList();
            }
            if (priority.HasValue)
            {
                tasks = tasks.Where(t => t.Priority == priority.Value).ToList();
            }
            if (!string.IsNullOrEmpty(department))
            {
                tasks = tasks.Where(t => t.DepartmentTasks.Any(dt => dt.Dept.DeptId == department)).ToList();
            }

            // Thiết lập phân trang cho danh sách task
            int pageSize = 6;
            int pageNumber = page ?? 1;
            ViewBag.Tasks = tasks.ToPagedList(pageNumber, pageSize);

            // Truyền dữ liệu bổ sung vào ViewBag
            ViewBag.ProjectId = id;
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
        TempData["Message"] = "Project not found.";
        TempData["MessageType"] = "error";
        return RedirectToAction("ProjectManage");
    }

    // Kiểm tra điều kiện phòng ban đã được giao nhiệm vụ
    var lockedDepartments = _projectService.GetDepartmentsWithAssignedTasks(ProjectId);
    var invalidDepartments = lockedDepartments.Except(SelectedDepartments).ToList();
    if (invalidDepartments.Any())
    {
        TempData["Message"] = "Không thể bỏ chọn các phòng ban đã được giao nhiệm vụ.";
        TempData["MessageType"] = "error";
        return RedirectToAction("ProjectManage");
    }

    try
    {
        _projectService.UpdateProject(project, SelectedDepartments);
        TempData["Message"] = "Cập nhật dự án thành công.";
        TempData["MessageType"] = "success";
    }
    catch (Exception ex)
    {
        TempData["Message"] = $"Lỗi khi cập nhật dự án: {ex.Message}";
        TempData["MessageType"] = "error";
    }

    return RedirectToAction("ProjectManage");
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
        public IActionResult DeleteProject(int projectId)
        {
            var project = _projectService.GetProjectById(projectId);
            if (project == null)
            {
                TempData["Message"] = "Dự án không tồn tại.";
                TempData["MessageType"] = "error";
                return RedirectToAction("ProjectManage");
            }

            // Kiểm tra nếu dự án có nhiệm vụ
            if (project.Tasks != null && project.Tasks.Any())
            {
                TempData["Message"] = "Không thể xóa dự án vì đã có nhiệm vụ được tạo.";
                TempData["MessageType"] = "error";
                return RedirectToAction("ProjectManage");
            }

            // Nếu dự án không có nhiệm vụ thì thực hiện xóa
            try
            {
                _projectService.DeleteProject(projectId);
                TempData["Message"] = "Dự án đã được xóa thành công.";
                TempData["MessageType"] = "success";
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"Lỗi khi xóa dự án: {ex.Message}";
                TempData["MessageType"] = "error";
            }

            return RedirectToAction("ProjectManage");
        }

        public IActionResult TaskDetail(string taskId)
        {
            var task = _taskService.GetTaskById(taskId);
            if (task == null) return NotFound("Task not found.");

            // Lấy subtasks kèm theo team của từng subtask
            var subtasks = _subtaskService.GetSubtasksByTaskId(taskId);

            // Tạo Dictionary để lưu danh sách nhân viên đảm nhận từng subtask
            var subtaskEmployeeMap = new Dictionary<string, List<Employee>>();

            foreach (var subtask in subtasks)
            {
                var employees = _subtaskService.GetEmployeesBySubtaskId(subtask.SubtaskId);
                subtaskEmployeeMap[subtask.SubtaskId] = employees;
            }

            ViewBag.Task = task;
            ViewBag.Subtasks = subtasks;
            ViewBag.SubtaskEmployeeMap = subtaskEmployeeMap; // Thêm Dictionary chứa danh sách nhân viên

            return View();
        }



    }

}
