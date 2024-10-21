using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskForge.Service;

namespace TaskForge.Controllers
{
    public class LeaderController : Controller
    {
        private readonly EmployeeService _employeeService;
        private readonly DropboxService _dropboxService;
        private readonly ProjectService _projectService;

        // Constructor duy nhất cho cả hai service
        public LeaderController(EmployeeService employeeService, DropboxService dropboxService, ProjectService projectService)
        {
            _employeeService = employeeService;
            _dropboxService = dropboxService;
            _projectService = projectService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Project()
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
    }
}
