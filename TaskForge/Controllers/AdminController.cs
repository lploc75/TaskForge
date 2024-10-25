using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskForge.Models;
using TaskForge.Service;
using X.PagedList;
using X.PagedList.Extensions;

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
        public IActionResult Credit(string accountId, string status, int? minCredits, int? maxCredits, decimal? minCash, decimal? maxCash, DateTime? startDate, DateTime? endDate, int? page)
        {
            // Lấy tất cả các CreditExchange từ Service
            var exchanges = _creditExchangeService.GetAllCreditExchanges();

            // Lọc theo AccountId nếu có
            if (!string.IsNullOrEmpty(accountId))
            {
                exchanges = exchanges.Where(e => e.AccountId.Contains(accountId)).ToList();
            }

            // Lọc theo trạng thái nếu có
            if (!string.IsNullOrEmpty(status))
            {
                exchanges = exchanges.Where(e => e.Status == status).ToList();
            }

            // Lọc theo khoảng CreditPointsUsed
            if (minCredits.HasValue)
            {
                exchanges = exchanges.Where(e => e.CreditPointsUsed >= minCredits).ToList();
            }
            if (maxCredits.HasValue)
            {
                exchanges = exchanges.Where(e => e.CreditPointsUsed <= maxCredits).ToList();
            }

            // Lọc theo khoảng CashAmount
            if (minCash.HasValue)
            {
                exchanges = exchanges.Where(e => e.CashAmount >= minCash).ToList();
            }
            if (maxCash.HasValue)
            {
                exchanges = exchanges.Where(e => e.CashAmount <= maxCash).ToList();
            }

            // Lọc theo khoảng ngày ExchangeDate
            if (startDate.HasValue)
            {
                exchanges = exchanges.Where(e => e.ExchangeDate >= startDate).ToList();
            }
            if (endDate.HasValue)
            {
                exchanges = exchanges.Where(e => e.ExchangeDate <= endDate).ToList();
            }

            // Thiết lập phân trang, mỗi trang có 10 phần tử
            int pageSize = 10;
            int pageNumber = (page ?? 1); // Nếu không có số trang, mặc định là 1

            // Áp dụng phân trang sau khi đã lọc
            var pagedExchanges = exchanges.ToPagedList(pageNumber, pageSize);

            // Trả về View với danh sách đã phân trang
            return View(pagedExchanges);
        }


        [HttpPost]
        public IActionResult UpdateExchangeStatus(int exchangeId, string status)
        {
            var exchange = _creditExchangeService.GetCreditExchangeById(exchangeId);
            if (exchange == null)
            {
                TempData["Error"] = "Giao dịch không tồn tại.";
                return RedirectToAction("Credit");
            }

            // Cập nhật trạng thái giao dịch theo trạng thái được truyền vào
            _creditExchangeService.UpdateCreditExchangeStatus(exchangeId, status);

            TempData["Success"] = "Trạng thái giao dịch đã được cập nhật thành công.";
            return RedirectToAction("Credit");
        }

    }
}
