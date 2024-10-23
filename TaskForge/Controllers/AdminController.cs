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
        public IActionResult EditAccount(string accountId, string Username, string Password, string Email, string PhoneNumber)
        {
            // Gọi service để cập nhật tài khoản
            _adminService.EditAccount(accountId, Username, Password, Email, PhoneNumber);
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

        // View team list with department ids using ViewBag
        public IActionResult Team()
        {
            // Lấy tất cả các team
            var teams = _adminService.GetAllTeams();

            // Lấy tất cả các department Id
            var departments = _departmentService.GetAllDepartments();

            // Truyền danh sách team vào ViewBag
            ViewBag.Teams = teams;

            // Truyền danh sách department Id vào ViewBag
            ViewBag.Departments = departments;

            return View();
        }

        // UC-29: CRUD Team
        [HttpPost]
        public IActionResult CreateTeam(string TeamName, DateOnly CreatedDate, int NumberOfMember, string DeptId)
        {
            // Gọi service để tạo team mới
            _teamService.CreateTeam(TeamName, CreatedDate, NumberOfMember, DeptId);
            // Thêm thông báo thành công
            TempData["SuccessMessage"] = "Team was created successfully!";
            // Chuyển hướng về trang danh sách team sau khi tạo xong
            return RedirectToAction("Team");
        }

        [HttpPost]
        public IActionResult EditTeam(string teamId, string TeamName, DateOnly CreatedDate, int NumberOfMember, string DeptId)
        {
            // Gọi service để cập nhật team
            _teamService.EditTeam(teamId, TeamName, CreatedDate, NumberOfMember, DeptId);
            // Thêm thông báo thành công
            TempData["SuccessMessage"] = "Team was updated successfully!";
            // Chuyển hướng về trang danh sách team
            return RedirectToAction("Team");
        }


        [HttpPost]
        public IActionResult DeleteTeam(string teamId)
        {
            _teamService.DeleteTeam(teamId);
            // Thêm thông báo thành công
            TempData["SuccessMessage"] = "Team was deleted successfully!";
            return RedirectToAction("Team");
        }
        // Hiển thị trang quản lý Team với bộ lọc
        [HttpGet]
        public IActionResult TeamFiltered(string deptId, int? numberOfTeam, DateOnly? createdDate)
        {
            // Lấy danh sách các team đã được lọc
            var teams = _teamService.GetTeamsWithFilters(deptId, numberOfTeam, createdDate);
            var departments = _departmentService.GetAllDepartments();

            // Truyền dữ liệu vào ViewBag
            ViewBag.Teams = teams;
            ViewBag.Departments = departments;

            return View("Team");
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
