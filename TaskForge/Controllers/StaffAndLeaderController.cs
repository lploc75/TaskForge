using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskForge.Service;
using TaskForge.Models;
using System.Collections.Generic;
using TaskForge.Repository;
using System.Security.Claims;
using X.PagedList.Extensions;

namespace TaskForge.Controllers
{
    [Authorize(Roles = "Staff, Leader")]  // Cho phép các vai trò Staff và Leader
    public class StaffandLeaderController : Controller
    {
        private readonly EmployeeService _employeeService;
        private readonly DropboxService _dropboxService;
        private readonly ProjectService _projectService;
        private readonly TaskService _taskService;
        private readonly NotificationService _notificationService;
        private readonly FeedbackService _feedbackService;
        // Constructor duy nhất cho cả hai service
        public StaffandLeaderController(EmployeeService employeeService, DropboxService dropboxService, 
            ProjectService projectService, TaskService taskService, NotificationService notificationService, FeedbackService feedbackService)
        {
            _employeeService = employeeService;
            _dropboxService = dropboxService;
            _projectService = projectService;
            _taskService = taskService;
            _notificationService = notificationService;
            _feedbackService = feedbackService;
        }

        public IActionResult Index()
        {
            string accountId = User.FindFirst("AccountId")?.Value;
            var recentNotifications = _notificationService.GetRecentNotifications(accountId, 5); // Lấy 5 thông báo gần nhất
            ViewData["RecentNotifications"] = recentNotifications; // Gửi thông báo vào ViewData
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
        public IActionResult LeaderViewStaffList(string accountId, string fullname, int? page)
        {
            // Lấy accountId của leader đang đăng nhập
            string leaderAccountId = User.FindFirst("AccountId")?.Value;

            // Lấy teamId của leader từ accountId
            string teamId = _employeeService.GetTeamIdByAccountId(leaderAccountId);

            // Lấy danh sách nhân viên có cùng teamId
            var staffs = _employeeService.GetStaffByTeamId(teamId).AsQueryable();

            // Lọc theo accountId nếu có
            if (!string.IsNullOrEmpty(accountId))
            {
                staffs = staffs.Where(s => s.AccountId.Contains(accountId));
            }

            // Lọc theo fullname nếu có
            if (!string.IsNullOrEmpty(fullname))
            {
                staffs = staffs.Where(s => s.Fullname.Contains(fullname));
            }

            // Thiết lập phân trang, mỗi trang có 10 phần tử
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            // Áp dụng phân trang
            var pagedStaffs = staffs.ToPagedList(pageNumber, pageSize);

            // Lưu giá trị lọc vào ViewBag để hiển thị lại trong form tìm kiếm
            ViewBag.AccountId = accountId;
            ViewBag.Fullname = fullname;

            // Truyền danh sách nhân viên đã phân trang vào Model
            return View(pagedStaffs);
        }


        public IActionResult LeaderAssignTask(string subtaskId, string subtaskName, string status, int? priority, int? difficulty, DateTime? startDate, DateTime? endDate, int? page)
        {
            // Lấy accountId của leader đang đăng nhập
            string accountId = User.FindFirst("AccountId")?.Value;

            // Lấy teamId của leader từ accountId
            string teamId = _employeeService.GetTeamIdByAccountId(accountId);

            // Lấy danh sách subtask thuộc team của leader
            var subtasks = _taskService.GetSubtasksByTeam(teamId);

            // Lọc theo Subtask ID nếu có
            if (!string.IsNullOrEmpty(subtaskId))
            {
                subtasks = subtasks.Where(s => s.SubtaskId.Contains(subtaskId)).ToList();
            }

            // Lọc theo Subtask Name nếu có
            if (!string.IsNullOrEmpty(subtaskName))
            {
                subtasks = subtasks.Where(s => s.SubtaskName.Contains(subtaskName)).ToList();
            }

            // Lọc theo trạng thái nếu có
            if (!string.IsNullOrEmpty(status))
            {
                subtasks = subtasks.Where(s => s.Status == status).ToList();
            }

            // Lọc theo Priority nếu có
            if (priority.HasValue)
            {
                subtasks = subtasks.Where(s => s.Priority == priority).ToList();
            }

            // Lọc theo Difficulty nếu có
            if (difficulty.HasValue)
            {
                subtasks = subtasks.Where(s => s.Difficulty == difficulty).ToList();
            }

            // Lọc theo khoảng ngày AssignmentDate
            if (startDate.HasValue)
            {
                subtasks = subtasks.Where(s => s.AssignmentDate >= startDate).ToList();
            }
            if (endDate.HasValue)
            {
                subtasks = subtasks.Where(s => s.Deadline <= endDate).ToList();
            }

            // Thiết lập phân trang, mỗi trang có 10 phần tử
            int pageSize = 10;
            int pageNumber = (page ?? 1); // Nếu không có số trang, mặc định là 1

            // Áp dụng phân trang sau khi đã lọc
            var pagedSubtasks = subtasks.ToPagedList(pageNumber, pageSize);

            // Trả về View với danh sách đã phân trang
            return View(pagedSubtasks);
        }
        [HttpPost]
        public IActionResult AssignSubtask(string subtaskId, string assignedTo)
        {
            string created_by = _employeeService.GetDepartmentHeadBySubtaskId(subtaskId);
            _taskService.AssignSubtask(subtaskId, assignedTo, created_by);
            return RedirectToAction("LeaderAssignTask");
        }

        [HttpPost]
        public IActionResult UnassignSubtask(string subtaskId)
        {
            _taskService.UnassignSubtask(subtaskId);
            return RedirectToAction("LeaderAssignTask");
        }

        public IActionResult Task()
        {
            // Lấy 'AccountId' từ các claims của người dùng hiện tại đã đăng nhập
            string accountId = User.FindFirst("AccountId")?.Value;
            var recentNotifications = _notificationService.GetRecentNotifications(accountId, 5); // Lấy 5 thông báo gần nhất
            ViewData["RecentNotifications"] = recentNotifications; // Gửi thông báo vào ViewData

            // Lấy danh sách các subtasks và personalTask được gán cho tài khoản dựa vào accountId,
            // nếu không tìm thấy subtasks nào, khởi tạo danh sách trống
            var subtasks = _employeeService.GetAssignedSubtasks(accountId);
            var personalTasks = _employeeService.GetPersonalTasks(accountId);

            // Sử dụng ViewBag để truyền dữ liệu
            ViewBag.Subtasks = subtasks;
            ViewBag.PersonalTasks = personalTasks;
            ViewBag.AccountId = accountId;
            // Trả về View và truyền danh sách subtasks đã gán vào Model
            return View();
        }
        public IActionResult Project()
        {
            var accountId = User.FindFirstValue("AccountId");
            var recentNotifications = _notificationService.GetRecentNotifications(accountId, 5); // Lấy 5 thông báo gần nhất
            ViewData["RecentNotifications"] = recentNotifications; // Gửi thông báo vào ViewData
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
        public IActionResult TaskFilter(string status, int? priority, int? difficulty, DateTime? assignmentDateMin, DateTime? assignmentDateMax, DateTime? deadlineMin, DateTime? deadlineMax, string submission, string taskId, string teamId, int? page)
        {
            string accountId = User.FindFirst("AccountId")?.Value;
            var recentNotifications = _notificationService.GetRecentNotifications(accountId, 5); // Lấy 5 thông báo gần nhất
            ViewData["RecentNotifications"] = recentNotifications; // Gửi thông báo vào ViewData

            var subtasks = _taskService.GetAllSubtasks();

            // Lọc theo các tiêu chí
            if (!string.IsNullOrEmpty(status))
                subtasks = subtasks.Where(s => s.Status == status).ToList();

            if (priority.HasValue)
                subtasks = subtasks.Where(s => s.Priority == priority).ToList();

            if (difficulty.HasValue)
                subtasks = subtasks.Where(s => s.Difficulty == difficulty).ToList();

            if (assignmentDateMin.HasValue)
                subtasks = subtasks.Where(s => s.AssignmentDate >= assignmentDateMin).ToList();

            if (assignmentDateMax.HasValue)
                subtasks = subtasks.Where(s => s.AssignmentDate <= assignmentDateMax).ToList();

            if (deadlineMin.HasValue)
                subtasks = subtasks.Where(s => s.Deadline >= deadlineMin).ToList();

            if (deadlineMax.HasValue)
                subtasks = subtasks.Where(s => s.Deadline <= deadlineMax).ToList();

            if (!string.IsNullOrEmpty(submission))
                subtasks = subtasks.Where(s => s.SubmissionDate.HasValue == (submission == "Yes")).ToList();

            if (!string.IsNullOrEmpty(taskId))
                subtasks = subtasks.Where(s => s.TaskId.Contains(taskId)).ToList();

            if (!string.IsNullOrEmpty(teamId))
                subtasks = subtasks.Where(s => s.TeamId.Contains(teamId)).ToList();
            
            // Thiết lập phân trang
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            var pagedSubtasks = subtasks.ToPagedList(pageNumber, pageSize);
            return View(pagedSubtasks);
        }
        public IActionResult PersonalTaskFilter(string status, int? priority, DateTime? assignmentDateMin, DateTime? assignmentDateMax, DateTime? deadlineMin, DateTime? deadlineMax, int? page)
        {
            var personalTasks = _taskService.GetAllPersonalTasks();

            // Lọc theo các tiêu chí
            if (!string.IsNullOrEmpty(status))
                personalTasks = personalTasks.Where(p => p.Status == status).ToList();

            if (priority.HasValue)
                personalTasks = personalTasks.Where(p => p.Priority == priority).ToList();

            if (assignmentDateMin.HasValue)
                personalTasks = personalTasks.Where(p => p.AssignmentDate >= assignmentDateMin).ToList();

            if (assignmentDateMax.HasValue)
                personalTasks = personalTasks.Where(p => p.AssignmentDate <= assignmentDateMax).ToList();

            if (deadlineMin.HasValue)
                personalTasks = personalTasks.Where(p => p.Deadline >= deadlineMin).ToList();

            if (deadlineMax.HasValue)
                personalTasks = personalTasks.Where(p => p.Deadline <= deadlineMax).ToList();

            // Thiết lập phân trang
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            var pagedPersonalTasks = personalTasks.ToPagedList(pageNumber, pageSize);
            return View(pagedPersonalTasks);
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
            var recentNotifications = _notificationService.GetRecentNotifications(accountId, 5); // Lấy 5 thông báo gần nhất
            ViewData["RecentNotifications"] = recentNotifications; // Gửi thông báo vào ViewData
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
        public IActionResult UpdateSubtaskStatus(string subtaskId, string status)
        {
            // Gọi service để cập nhật trạng thái task
            var result = _employeeService.UpdateSubtaskStatus(subtaskId, status);
            if (result)
            {
                return RedirectToAction("Task");
            }
            return RedirectToAction("Error", "Home");
        }
        [HttpPost]
        public IActionResult RejectSubtask(string subtaskId)
        {
            _taskService.RejectSubtaskAssignment(subtaskId);
            return RedirectToAction("Task"); // Redirect to the appropriate view
        }
        [HttpPost]
        public IActionResult UpdatePersonalTaskStatus(string PtaskId, string status)
        {
            if(status.Equals("In Progress"))
            {
                status = "Completed";
            }
            else
            {
                status = "In Progress";
            }
            // Gọi service để cập nhật trạng thái task
            var result = _employeeService.UpdatePersonalTaskkStatus(PtaskId, status);
            // Thêm thông báo thành công
            TempData["SuccessMessage"] = "Personal task's status was updated successfully!";
            if (result)
            {
                return RedirectToAction("Task");
            }
            return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        public IActionResult CreatePersonalTask(string PtaskName, DateTime assignedDate, DateTime deadline, int priority, string description)
        {
            // Lấy AccountId của người dùng hiện tại
            var accountId = User.FindFirstValue("AccountId");
            if (accountId == null)
            {
                return RedirectToAction("Login", "Account"); // Nếu không có accountId, chuyển hướng tới trang Login
            }

            if (ModelState.IsValid)
            {
                // Sinh ID ngẫu nhiên cho PersonalTask
                string ptask_id = GenerateRandomPtaskId();

                // Tạo đối tượng PersonalTask mới
                var ptask = new PersonalTask
                {
                    PtaskId = ptask_id,              // Gán PtaskId ngẫu nhiên
                    AccountId = accountId,           // Gán AccountId của người dùng
                    PtaskName = PtaskName,           // Lấy tên task từ form
                    Status = "In Progress",           // Trạng thái mặc định khi tạo task là "In Progress"
                    Priority = priority,             // Gán độ ưu tiên từ form
                    AssignmentDate = assignedDate,   // Gán ngày tạo task từ form
                    Deadline = deadline,             // Gán hạn deadline từ form
                    Description = description       // Gán mô tả từ form
                };

                // Gọi service hoặc repository để lưu PersonalTask vào database
                _taskService.AddPersonalTask(ptask);
                // Thêm thông báo thành công
                TempData["SuccessMessage"] = "Personal task created successfully!";
                // Sau khi tạo task, chuyển hướng người dùng về trang Task
                return RedirectToAction("Task");
            }

            // Nếu model không hợp lệ, quay lại trang và hiển thị lại form
            return View();
        }

        [HttpPost]
        public IActionResult UpdatePersonalTask(string PtaskId, string PtaskName, string Description, DateTime assignedDate, DateTime deadline, int priority)
        {
            // Lấy Personal Task từ Service
            var personalTask = _taskService.GetPersonalTaskById(PtaskId);
            if (personalTask == null)
            {
                return NotFound("Personal Task not found.");
            }

            // Cập nhật thông tin của Personal Task
            personalTask.PtaskName = PtaskName;
            personalTask.Description = Description;
            personalTask.AssignmentDate = assignedDate;
            personalTask.Deadline = deadline;
            personalTask.Priority = priority;

            // Gọi Service để cập nhật task
            _taskService.UpdatePersonalTask(personalTask);
            // Thêm thông báo thành công
            TempData["SuccessMessage"] = "Personal task updated successfully!";
            // Chuyển hướng lại trang quản lý sau khi cập nhật
            return RedirectToAction("Task");
        }

        [HttpPost]
        public IActionResult DeletePersonalTask(string PtaskId)
        {
            var task = _taskService.GetPersonalTaskById(PtaskId);
            if (task == null)
            {
                TempData["ErrorMessage"] = "Task not found.";
                return RedirectToAction("Task");
            }

            // Gọi service để xóa task
            _taskService.DeletePersonalTask(PtaskId);

            // Thông báo xóa thành công
            TempData["SuccessMessage"] = "Task deleted successfully!";
            return RedirectToAction("Task");
        }

        // Phương thức tạo ID mới ngẫu nhiên cho PersonalTask
        public string GenerateRandomPtaskId()
        {
            string accountId = User.FindFirst("AccountId")?.Value;
            var recentNotifications = _notificationService.GetRecentNotifications(accountId, 5); // Lấy 5 thông báo gần nhất
            ViewData["RecentNotifications"] = recentNotifications; // Gửi thông báo vào ViewData
            // Tạo đối tượng Random
            Random random = new Random();

            // Tạo số ngẫu nhiên trong khoảng từ 1 đến 999
            int randomNumber = random.Next(1, 1000); // Số ngẫu nhiên từ 1 đến 999

            // Trả về ID mới với định dạng "PT" + số ngẫu nhiên với 3 chữ số
            return "PT" + randomNumber.ToString("D3"); // D3 định dạng số thành 3 chữ số (ví dụ: "PT045")
        }
        // Action để hiển thị trang upload file
        [HttpGet]
        public async Task<IActionResult> UploadFile(string code, string subtaskId)
        {
            if (!string.IsNullOrEmpty(code))
            {
                // Gọi Dropbox API để trao đổi mã lấy access token
                await _dropboxService.ExchangeCodeForTokenAsync(code);

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
                AccountId = accountId,
                SubtaskId = subtaskId
            };

            // Tải tệp lên Dropbox
            var dropboxFilePath = await _dropboxService.UploadFileAsync(tempFilePath, file.FileName, accountId, subtaskId, newFile);

            // Xóa tệp tạm thời sau khi tải lên
            if (System.IO.File.Exists(tempFilePath))
            {
                System.IO.File.Delete(tempFilePath);
            }

            ViewBag.Message = "File uploaded successfully to Dropbox and saved to database.";
            ViewBag.SubtaskId = subtaskId;
            return View();
        }
        // Exchange Page Action
        public IActionResult Exchange()
        {
            string accountId = User.FindFirst("AccountId")?.Value;
            var recentNotifications = _notificationService.GetRecentNotifications(accountId, 5); // Lấy 5 thông báo gần nhất
            ViewData["RecentNotifications"] = recentNotifications; // Gửi thông báo vào ViewData

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

            // Truyền trực tiếp số điểm và số tiền qua ViewData
            ViewData["AvailableCredits"] = availableCredits;
            ViewData["CashEquivalent"] = cashEquivalent;

            return View();
        }

        [HttpPost]
        public IActionResult SubmitExchange(int? pointsToRedeem, int availableCredits)
        {
            string accountId = User.FindFirst("AccountId")?.Value;

            if (string.IsNullOrEmpty(accountId))
            {
                return RedirectToAction("Error", "Home");
            }

            // Kiểm tra nếu người dùng chưa nhập số điểm
            if (!pointsToRedeem.HasValue)
            {
                TempData["Message"] = "Please enter the points you want to redeem.";
                ViewData["AvailableCredits"] = availableCredits;
                ViewData["CashEquivalent"] = availableCredits * 0.5m;
                return View("Exchange");
            }

            if (pointsToRedeem < 100 || pointsToRedeem > availableCredits)
            {
                TempData["Message"] = "The number of points entered is invalid.";
                ViewData["AvailableCredits"] = availableCredits;
                ViewData["CashEquivalent"] = availableCredits * 0.5m;
                return View("Exchange");
            }

            var exchangeId = _employeeService.RedeemCredits(accountId, pointsToRedeem.Value);

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
            var exchange = _employeeService.GetCreditExchangeById(exchangeId);

            if (exchange == null || exchange.Account == null)
            {
                return RedirectToAction("Error", "Home");
            }

            // Truyền dữ liệu trực tiếp qua ViewData
            ViewData["AvailableCredits"] = exchange.Account.CreditPoints ?? 0;
            ViewData["CashEquivalent"] = exchange.CashAmount;
            ViewData["ExchangeId"] = exchange.ExchangeId;

            return View();
        }
        public IActionResult Feedback()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SubmitFeedback(string context)
        {
            if (string.IsNullOrWhiteSpace(context))
            {
                TempData["Message"] = "Please enter your feedback before submitting.";
                return RedirectToAction("Feedback");
            }

            string accountId = User.FindFirst("AccountId")?.Value;

            // Lấy feedback_id cao nhất hiện tại và tăng lên 1
            int newFeedbackId = _feedbackService.GetNextFeedbackId();

            // Tạo đối tượng feedback với feedback_id mới
            var feedback = new Feedback
            {
                FeedbackId = newFeedbackId,
                Context = context,
                DateSubmitted = DateTime.Now,
                AccountId = accountId
            };

            _feedbackService.CreateFeedback(feedback);
            TempData["Message"] = "Thank you for your feedback!";

            return RedirectToAction("Feedback");
        }

    }
}
