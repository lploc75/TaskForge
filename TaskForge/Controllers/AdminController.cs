using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskForge.Models;
using TaskForge.Service;

namespace TaskForge.Controllers
{

    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly EmployeeService _employeeService;
        private readonly AdminService _adminService;
        private readonly DepartmentService _departmentService;
        private readonly CreditExchangeService _creditExchangeService;
        private readonly FeedbackService _feedbackService;
        private readonly TeamService _teamService;
        private readonly NotificationService _notificationService;

        public AdminController(EmployeeService employeeService, AdminService adminService, DepartmentService departmentService, CreditExchangeService creditExchangeService,
                               FeedbackService feedbackService, TeamService teamService, NotificationService notificationService)
        {
            _employeeService = employeeService;
            _adminService = adminService;
            _departmentService = departmentService;
            _creditExchangeService = creditExchangeService;
            _feedbackService = feedbackService;
            _teamService = teamService;
            _notificationService = notificationService;
        }

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

        // UC-25: View Account List
        public IActionResult ViewAccountList()
        {
            var accounts = _adminService.GetAllAccounts();
            return View(accounts);
        }

        // UC-28: CRUD Account
        [HttpPost]
        public IActionResult CreateAccount(string Username, string Password, string Email, string PhoneNumber)
        {
            // Gọi service để tạo tài khoản mới
            _adminService.CreateAccount(Username, Password, Email, PhoneNumber);
            // Thêm thông báo thành công
            TempData["SuccessMessage"] = "Account was created successfully!";
            // Chuyển hướng về trang danh sách tài khoản sau khi tạo xong
            return RedirectToAction("ViewAccountList");
        }



        [HttpPost]
        public IActionResult EditAccount(string accountId, string Username, string Email, string PhoneNumber)
        {
            // Gọi service để cập nhật tài khoản
            _adminService.EditAccount(accountId, Username, Email, PhoneNumber);
            // Thêm thông báo thành công
            TempData["SuccessMessage"] = "Account was updated successfully!";
            // Chuyển hướng về trang danh sách tài khoản
            return RedirectToAction("ViewAccountList");
        }


        public IActionResult DeleteAccount(string accountId)
        {
            // Gọi service để xóa tài khoản
            _adminService.DeleteAccount(accountId);
            // Thêm thông báo thành công
            TempData["SuccessMessage"] = "Account was deleted successfully!";
            // Chuyển hướng về trang danh sách tài khoản
            return RedirectToAction("ViewAccountList");
        }

        // UC-27: View Department List
        public IActionResult ViewDepartmentList()
        {
            var departments = _departmentService.GetAllDepartments();
            return View(departments);
        }

        // UC-28: CRUD Department
        [HttpPost]
        public IActionResult CreateDepartment(Department newDepartment)
        {
            if (ModelState.IsValid)
            {
                _departmentService.CreateDepartment(newDepartment);
                return RedirectToAction("ViewDepartmentList");
            }
            return View(newDepartment);
        }

        [HttpPost]
        public IActionResult EditDepartment(Department updatedDepartment)
        {
            if (ModelState.IsValid)
            {
                _departmentService.UpdateDepartment(updatedDepartment);
                return RedirectToAction("ViewDepartmentList");
            }
            return View(updatedDepartment);
        }

        [HttpPost]
        public IActionResult DeleteDepartment(string deptId)
        {
            _departmentService.DeleteDepartment(deptId);
            return RedirectToAction("ViewDepartmentList");
        }

        // UC-29: View Credit Exchange List (Xem danh sách trao đổi tín dụng)
        public IActionResult ViewCreditExchangeList()
        {
            var exchanges = _creditExchangeService.GetAllCreditExchanges();
            return View(exchanges);
        }

        // UC-30: Approve Credit Exchange (Phê duyệt trao đổi tín dụng)
        [HttpPost]
        public IActionResult ApproveCreditExchange(int exchangeId)
        {
            _creditExchangeService.ApproveCreditExchange(exchangeId);
            return RedirectToAction("ViewCreditExchangeList");
        }

        // UC-31: View Feedback List
        public IActionResult ViewFeedbackList()
        {
            var feedbacks = _feedbackService.GetAllFeedbacks();
            return View(feedbacks);
        }

        // UC-32: CRD Feedback (Create, Read, Delete Feedback)
        [HttpPost]
        public IActionResult CreateFeedback(Feedback newFeedback)
        {
            if (ModelState.IsValid)
            {
                _feedbackService.CreateFeedback(newFeedback);
                return RedirectToAction("ViewFeedbackList");
            }
            return View(newFeedback);
        }

        [HttpPost]
        public IActionResult DeleteFeedback(int feedbackId)
        {
            _feedbackService.DeleteFeedback(feedbackId);
            return RedirectToAction("ViewFeedbackList");
        }

        // UC-33: View Team List
        public IActionResult ViewTeamList()
        {
            var teams = _teamService.GetAllTeams();
            return View(teams);
        }

        // UC-34: CRUD Team
        [HttpPost]
        public IActionResult CreateTeam(Team newTeam)
        {
            if (ModelState.IsValid)
            {
                _teamService.CreateTeam(newTeam);
                return RedirectToAction("ViewTeamList");
            }
            return View(newTeam);
        }

        [HttpPost]
        public IActionResult EditTeam(Team updatedTeam)
        {
            if (ModelState.IsValid)
            {
                _teamService.UpdateTeam(updatedTeam);
                return RedirectToAction("ViewTeamList");
            }
            return View(updatedTeam);
        }

        [HttpPost]
        public IActionResult DeleteTeam(string teamId)
        {
            _teamService.DeleteTeam(teamId);
            return RedirectToAction("ViewTeamList");
        }

        // UC-35: Notify for User
        [HttpPost]
        public IActionResult NotifyUser(string userId, string message)
        {
            var user = _adminService.GetAccountById(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Ví dụ gửi email hoặc hiển thị thông báo
            _notificationService.SendNotification(user.Email, message);

            return RedirectToAction("Index");
        }
    }
}
