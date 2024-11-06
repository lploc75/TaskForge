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
        private readonly ProjectService _projectService;
        private readonly TaskService _taskService;
        private readonly NotificationService _notificationService;
        private readonly FeedbackService _feedbackService;
        private readonly SubtaskService _subtaskService;
        private readonly CreditExchangeService _creditExchangeService;

        // Constructor duy nhất cho cả hai service
        public StaffandLeaderController(EmployeeService employeeService, ProjectService projectService, TaskService taskService, NotificationService notificationService, FeedbackService feedbackService, SubtaskService subtaskService, CreditExchangeService creditExchangeService)
        {
            _employeeService = employeeService;
            _projectService = projectService;
            _taskService = taskService;
            _notificationService = notificationService;
            _feedbackService = feedbackService;
            _subtaskService = subtaskService;
            _creditExchangeService = creditExchangeService;
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

            // Gọi phương thức trong Service để lấy teamId của leader
            string teamId = _employeeService.GetTeamIdByAccountId(leaderAccountId);

            // Thiết lập phân trang, mỗi trang có 10 phần tử
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            // Gọi phương thức trong Service để lấy danh sách nhân viên đã lọc và phân trang
            var pagedStaffs = _employeeService.GetStaffByTeamIdWithFilters(teamId, accountId, fullname, pageNumber, pageSize);

            // Lưu giá trị lọc vào ViewBag để hiển thị lại trong form tìm kiếm
            ViewBag.AccountId = accountId;
            ViewBag.Fullname = fullname;

            // Truyền danh sách nhân viên đã phân trang vào Model
            return View(pagedStaffs);
        }
        public IActionResult LeaderAssignTask(string subtaskId, string subtaskName, string status, int? priority, int? difficulty,
                                        DateTime? startDate, DateTime? endDate, int? page)
        {
            // Lấy accountId của leader đang đăng nhập
            string leaderAccountId = User.FindFirst("AccountId")?.Value;

            // Lấy teamId của leader
            string teamId = _employeeService.GetTeamIdByAccountId(leaderAccountId);

            // Lấy danh sách nhân viên và truyền vào ViewBag
            var staff = _employeeService.GetStaffByTeamId(teamId);
            ViewBag.Employees = staff;

            // Lấy tất cả SubtaskAssignments
            var subtaskAssignments = _taskService.GetAllSubtaskAssignments();
            ViewBag.SubtaskAssignments = subtaskAssignments;

            // Thiết lập phân trang, mỗi trang có 10 phần tử
            int pageSize = 10;
            int pageNumber = page ?? 1;

            // Lấy danh sách subtasks đã lọc và phân trang
            var pagedSubtasks = _taskService.GetFilteredSubtasks(teamId, subtaskId, subtaskName, status, priority, difficulty, startDate, endDate, pageNumber, pageSize);

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

            ViewBag.OngoingProjects = ongoingProjects;
            ViewBag.CompletedProjects = completedProjects;
            ViewBag.CancelledProjects = cancelledProjects;

            return View();
        }

        public IActionResult TaskFilter(string status, int? priority, int? difficulty, DateTime? assignmentDateMin, DateTime? assignmentDateMax, DateTime? deadlineMin, DateTime? deadlineMax, string submission, string taskId, string teamId, int? page)
        {
            string accountId = User.FindFirst("AccountId")?.Value;
            var recentNotifications = _notificationService.GetRecentNotifications(accountId, 5); // Lấy 5 thông báo gần nhất
            ViewData["RecentNotifications"] = recentNotifications; // Gửi thông báo vào ViewData

            // Gọi Service để lấy danh sách subtasks đã lọc và phân trang
            int pageNumber = page ?? 1;
            int pageSize = 10;
            var pagedSubtasks = _subtaskService.GetFilteredSubtasks(status, priority, difficulty, assignmentDateMin, assignmentDateMax, deadlineMin, deadlineMax, submission, taskId, teamId, pageNumber, pageSize);

            // Truyền lại các giá trị vào ViewData để giữ lại sau khi load lại trang
            ViewData["Status"] = status;
            ViewData["Priority"] = priority;
            ViewData["Difficulty"] = difficulty;
            ViewData["AssignmentDateMin"] = assignmentDateMin;
            ViewData["AssignmentDateMax"] = assignmentDateMax;
            ViewData["DeadlineMin"] = deadlineMin;
            ViewData["DeadlineMax"] = deadlineMax;
            ViewData["Submission"] = submission;
            ViewData["TaskId"] = taskId;
            ViewData["TeamId"] = teamId;

            return View(pagedSubtasks);
        }

        public IActionResult PersonalTaskFilter(string status, int? priority, DateTime? assignmentDateMin, DateTime? assignmentDateMax, DateTime? deadlineMin, DateTime? deadlineMax, int? page)
        {
            string accountId = User.FindFirst("AccountId")?.Value;
            var recentNotifications = _notificationService.GetRecentNotifications(accountId, 5); // Lấy 5 thông báo gần nhất
            ViewData["RecentNotifications"] = recentNotifications; // Gửi thông báo vào ViewData

            // Gọi Service để lấy danh sách personal tasks đã lọc và phân trang
            int pageNumber = page ?? 1;
            int pageSize = 10;
            var pagedPersonalTasks = _taskService.GetFilteredPersonalTasks(status, priority, assignmentDateMin, assignmentDateMax, deadlineMin, deadlineMax, pageNumber, pageSize);

            // Truyền lại các giá trị vào ViewData để giữ lại sau khi load lại trang
            ViewData["Status"] = status;
            ViewData["Priority"] = priority;
            ViewData["AssignmentDateMin"] = assignmentDateMin;
            ViewData["AssignmentDateMax"] = assignmentDateMax;
            ViewData["DeadlineMin"] = deadlineMin;
            ViewData["DeadlineMax"] = deadlineMax;

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
            var result = _subtaskService.UpdateSubtaskStatus(subtaskId, status);
            if (result)
            {
                TempData["SuccessMessage"] = "task's status was updated successfully!";
                return RedirectToAction("Task");
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update task status!";
                return RedirectToAction("Task");
            }
        }

        [HttpPost]
        public IActionResult RejectSubtask(string subtaskId)
        {
            if (_subtaskService.RejectSubtaskAssignment(subtaskId))
            {
                TempData["SuccessMessage"] = "Task's status was updated successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update task status!";
            }
            return RedirectToAction("Task");
        }

        [HttpPost]
        public IActionResult UpdatePersonalTaskStatus(string PtaskId, string status)
        {
            // Gọi service để cập nhật trạng thái
            var result = _taskService.UpdatePersonalTaskStatus(PtaskId, status);

            if (result)
            {
                // Thêm thông báo thành công
                TempData["SuccessMessage"] = "Personal task's status was updated successfully!";
                return RedirectToAction("Task");
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update personal's task status!";
                return RedirectToAction("Task");
            }
        }

        [HttpPost]
        public IActionResult CreatePersonalTask(string PtaskName, DateTime assignedDate, DateTime deadline, int priority, string description)
        {
            // Lấy AccountId của người dùng hiện tại
            var accountId = User.FindFirstValue("AccountId");
            // Kiểm tra nếu accountId là null
            if (string.IsNullOrEmpty(accountId))
            {
                TempData["ErrorMessage"] = "Failed to create personal task. User not authenticated.";
                return RedirectToAction("Task");
            }

            //Kiểm tra nếu assignedDate sau deadline
            if (assignedDate >= deadline)
            {
                TempData["ErrorMessage"] = "Invalid input, assigned date cannot be equal or after the deadline";
                return RedirectToAction("Task");
            }

            var result = _taskService.CreatePersonalTask(PtaskName, assignedDate, deadline, priority, description, accountId);

            if (!result)
            {
                TempData["ErrorMessage"] = "Failed to create personal task. Please try again.";
                return RedirectToAction("Task");
            }

            TempData["SuccessMessage"] = "Personal task created successfully!";
            return RedirectToAction("Task");
        }

        [HttpPost]
        public IActionResult UpdatePersonalTask(string PtaskId, string PtaskName, string Description, DateTime assignedDate, DateTime deadline, int priority)
        {
            // Kiểm tra nếu assignedDate sau deadline
            if (assignedDate >= deadline)
            {
                TempData["ErrorMessage"] = "Invalid input, assigned date cannot be equal or after the deadline";
                return RedirectToAction("Task");
            }

            // Gọi service để cập nhật thông tin Personal Task
            var result = _taskService.UpdatePersonalTask(PtaskId, PtaskName, Description, assignedDate, deadline, priority);

            if (!result)
            {
                TempData["ErrorMessage"] = "Failed to update personal task. Please try again.";
                return RedirectToAction("Task");
            }

            TempData["SuccessMessage"] = "Personal task updated successfully!";
            return RedirectToAction("Task");
        }

        [HttpPost]
        public IActionResult DeletePersonalTask(string PtaskId)
        {
            // Gọi service để xóa task
            var result = _taskService.DeletePersonalTask(PtaskId);

            // Hiển thị thông báo phù hợp
            if (result)
            {
                TempData["SuccessMessage"] = "Task deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Task not found.";
            }

            return RedirectToAction("Task");
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

            if (!pointsToRedeem.HasValue)
            {
                TempData["Message"] = "Please enter the points you want to redeem.";
                ViewData["AvailableCredits"] = availableCredits;
                ViewData["CashEquivalent"] = availableCredits * 0.5m;
                return View("Exchange");
            }

            var result = _employeeService.SubmitExchange(accountId, pointsToRedeem.Value, availableCredits);

            if (!result.Success)
            {
                TempData["Message"] = result.Message;
                ViewData["AvailableCredits"] = availableCredits;
                ViewData["CashEquivalent"] = result.CashEquivalent;
                return View("Exchange");
            }

            return RedirectToAction("ExchangeConfirmation", new { exchangeId = result.ExchangeId });
        }

        public IActionResult ExchangeConfirmation(int exchangeId)
        {
            var result = _employeeService.GetExchangeConfirmation(exchangeId);

            if (!result.Success)
            {
                return RedirectToAction("Error", "Home");
            }

            ViewData["AvailableCredits"] = result.AvailableCredits;
            ViewData["CashEquivalent"] = result.CashEquivalent;
            ViewData["ExchangeId"] = result.Exchange.ExchangeId;

            return View();
        }
        public IActionResult ExchangeHistory(string status, int? minCredits, int? maxCredits, decimal? minCash, decimal? maxCash, DateTime? startDate, DateTime? endDate, int? page)
        {
            string accountId = User.FindFirst("AccountId")?.Value;
            Console.WriteLine("Current user's AccountId: " + accountId);
            // Giới hạn chỉ có thể xem lịch sử của chính mình
            if (string.IsNullOrEmpty(accountId))
            {
                Console.WriteLine("AccountId is null or empty"); // Ghi log cho việc kiểm tra accountId
                return RedirectToAction("Error", "Home");
            }

            var exchanges = _creditExchangeService.FilterCreditExchanges2(accountId, status, minCredits, maxCredits, minCash, maxCash, startDate, endDate);

            int pageSize = 10;
            int pageNumber = page ?? 1;
            var pagedExchanges = exchanges.ToPagedList(pageNumber, pageSize);

            return View(pagedExchanges);
        }

        public IActionResult Feedback()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SubmitFeedback(string context)
        {
            string accountId = User.FindFirst("AccountId")?.Value;

            var message = _feedbackService.SubmitFeedback(accountId, context);
            TempData["Message"] = message;

            return RedirectToAction("Feedback");
        }


        // Action hiển thị các bình luận cho một subtask
        public IActionResult Comment(string subtaskId)
        {
            if (string.IsNullOrEmpty(subtaskId))
            {
                return RedirectToAction("Error", "Home"); // Chuyển hướng nếu subtaskId không hợp lệ
            }

            var comments = _employeeService.GetCommentsBySubtaskId(subtaskId);
            ViewBag.SubtaskId = subtaskId;
            ViewBag.Comments = comments;
            return View("Comment");
        }

        // Action thêm bình luận cho một subtask
        [HttpPost]
        public IActionResult AddComment(string subtaskId, string commentText)
        {
            string accountId = User.FindFirst("AccountId")?.Value;
            if (!string.IsNullOrEmpty(accountId) && !string.IsNullOrEmpty(subtaskId) && !string.IsNullOrEmpty(commentText))
            {
                _employeeService.AddComment(accountId, subtaskId, commentText);
            }
            return RedirectToAction("Comment", new { subtaskId });
        }

        // Action xóa bình luận của một subtask
        [HttpPost]
        public IActionResult DeleteComment(string commentId, string subtaskId)
        {
            if (!string.IsNullOrEmpty(commentId))
            {
                _employeeService.DeleteComment(commentId);
            }
            return RedirectToAction("Comment", new { subtaskId });
        }

        public IActionResult Evaluate(string subtaskId)
        {
            var subtask = _subtaskService.GetSubtaskById(subtaskId);

            if (subtask == null || subtask.Status != "Pending")
            {
                return RedirectToAction("LeaderAssignTask");
            }

            return View(subtask);
        }

        [HttpPost]
        public IActionResult SubmitEvaluation(string subtaskId, string approvalStatus, string evaluationComment, int? teamworkRating, int? timelinessRating, int? kpiRating)
        {
            if (!teamworkRating.HasValue || !timelinessRating.HasValue || !kpiRating.HasValue)
            {
                ModelState.AddModelError("", "Please provide all ratings.");
                return RedirectToAction("Evaluate", new { subtaskId });
            }

            var subtask = _subtaskService.GetSubtaskById(subtaskId);

            if (subtask == null)
            {
                return RedirectToAction("LeaderAssignTask");
            }

            if (approvalStatus == "Assign")
            {
                subtask.Status = "Completed";
            }
            else if (approvalStatus == "Not Assign")
            {
                subtask.Status = "In Progress";
            }

            var evaluation = new SubtaskEvaluation
            {
                SubtaskId = subtaskId,
                EvaluationDate = DateTime.Now,
                Comment = approvalStatus == "Assign" ? evaluationComment : null,
                TeamworkRating = teamworkRating.Value,
                TimelinessRating = timelinessRating.Value,
                KpiRating = kpiRating.Value
            };

            _subtaskService.SaveEvaluation(evaluation);
            _subtaskService.UpdateSubtask(subtask);

            return RedirectToAction("LeaderAssignTask");
        }

    }
}
