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

        private readonly AccountService _accountService;
        private readonly AccountRepository _accountRepository;
        private readonly DropboxService _dropboxService;

        public StaffController(EmployeeService employeeService, AccountService accountService, AccountRepository accountRepository, DropboxService dropboxService)
        {
            _employeeService = employeeService;
            _accountService = accountService;
            _accountRepository = accountRepository;
            _dropboxService = dropboxService;

        }
        // Exchange Page Action
        public IActionResult Exchange()
        {
            string accountId = User.FindFirst("AccountId")?.Value;

            if (string.IsNullOrEmpty(accountId))
            {
                return RedirectToAction("Error", "Home");
            }

            // Get available credit points for the logged-in user
            StaffAndLeader staff = _employeeService.GetStaffByAccountId(accountId);
            if (staff == null || staff.CreditPoints == null)
            {
                return RedirectToAction("Error", "Home");
            }

            int availableCredits = staff.CreditPoints ?? 0;
            decimal cashEquivalent = availableCredits * 0.5m; // Assuming 1 credit = $0.50

            // ViewModel for the Exchange view
            var model = new ExchangeViewModel
            {
                AvailableCredits = availableCredits,
                CashEquivalent = cashEquivalent
            };

            return View(model);
        }

        // Action to Redeem Credits
        [HttpPost]
        public IActionResult RedeemCredits(int pointsToRedeem)
        {
            string accountId = User.FindFirst("AccountId")?.Value;

            if (string.IsNullOrEmpty(accountId))
            {
                return RedirectToAction("Error", "Home");
            }

            if (pointsToRedeem < 100)
            {
                ModelState.AddModelError("", "Minimum of 100 points must be redeemed at a time.");
                return View("Exchange");
            }

            var exchangeId = _accountService.RedeemCredits(accountId, pointsToRedeem);

            if (exchangeId != 0)
            {
                return RedirectToAction("ExchangeConfirmation", new { exchangeId });
            }
            else
            {
                ModelState.AddModelError("", "Failed to redeem credits.");
                return View("Exchange");
            }
        }
        [HttpPost]
        public IActionResult SubmitExchange(int pointsToRedeem)
        {
            string accountId = User.FindFirst("AccountId")?.Value;

            if (string.IsNullOrEmpty(accountId))
            {
                return RedirectToAction("Error", "Home");
            }

            if (pointsToRedeem < 100)
            {
                ModelState.AddModelError("", "Minimum of 100 points must be redeemed at a time.");
                return View("Exchange");
            }

            var exchangeId = _accountService.RedeemCredits(accountId, pointsToRedeem);

            if (exchangeId != 0)
            {
                return RedirectToAction("ExchangeConfirmation", new { exchangeId });
            }
            else
            {
                ModelState.AddModelError("", "Failed to redeem credits.");
                return View("Exchange");
            }
        }
        public IActionResult ExchangeConfirmation(int exchangeId)
        {
            var exchange = _accountRepository.GetCreditExchangeById(exchangeId);

            if (exchange == null || exchange.Account == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var model = new ExchangeViewModel
            {
                AvailableCredits = exchange.Account.CreditPoints ?? 0,
                CashEquivalent = exchange.CashAmount,
                ExchangeId = exchange.ExchangeId // Set ExchangeId for display
            };

            return View(model);
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
        [HttpGet]
        public async Task<IActionResult> FilteredTasks(string status, string priority, string difficulty, DateTime? deadline)
        {
            var filteredTasks = await _employeeService.GetFilteredTasksAsync(status, priority, difficulty, deadline);
            return View("FilteredTasks", filteredTasks); // Return filtered tasks to the view
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

        // Action để hiển thị trang upload file
        [HttpGet]
        public async Task<IActionResult> UploadFile(string code, string subtaskId)
        {
            if (!string.IsNullOrEmpty(code))
            {
                // Gọi Dropbox API để trao đổi mã lấy access token
                await _dropboxService.ExchangeCodeForTokenAsync(code);

                // Sau đó tiếp tục tải file nếu cần
            }

            ViewBag.SubtaskId = subtaskId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file, string subtaskId)
        {
            if (file == null || file.Length == 0)
            {
                ViewBag.Message = "No file selected.";
                return View();
            }

            // Lấy AccountId từ claim của người dùng đã đăng nhập
            var accountId = User.FindFirst("AccountId")?.Value;

            if (string.IsNullOrEmpty(accountId))
            {
                ViewBag.Message = "Could not determine account ID.";
                return View();
            }

            // Kiểm tra nếu access token hết hạn hoặc chưa tồn tại
            var accessToken = HttpContext.Session.GetString("DropboxAccessToken");
            if (string.IsNullOrEmpty(accessToken))
            {
                accessToken = await _dropboxService.RefreshAccessTokenAsync();
                if (string.IsNullOrEmpty(accessToken))
                {
                    return RedirectToAction("AuthorizeDropbox", "Dropbox");
                }
            }

            // Lưu tệp tạm thời lên server
            var tempFilePath = Path.Combine(Path.GetTempPath(), file.FileName);
            using (var stream = new FileStream(tempFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Tạo đối tượng File để lưu thông tin tệp
            var newFile = new Models.File
            {
                FileId = Guid.NewGuid().ToString(),
                FileName = file.FileName,
                UploadDate = DateOnly.FromDateTime(DateTime.Now),
                SubtaskId = subtaskId,
                AccountId = accountId
            };

            // Tải tệp lên Dropbox
            var dropboxFilePath = await _dropboxService.UploadFileAsync(tempFilePath, file.FileName, accountId, subtaskId, newFile);

            // Xóa tệp tạm thời sau khi tải lên
            if (System.IO.File.Exists(tempFilePath))
            {
                System.IO.File.Delete(tempFilePath);
            }

            ViewBag.Message = "File uploaded successfully to Dropbox and saved to database.";

            return View();
        }

    }
}
