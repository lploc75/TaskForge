using TaskForge.Models;
using TaskForge.Repository;
using X.PagedList;
using X.PagedList.Extensions;

namespace TaskForge.Service
{
    public class EmployeeService
    {
        private readonly EmployeeRepository _employeeRepository;

        public EmployeeService(EmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public CreditExchange GetCreditExchangeById(int exchangeId)
        {
            return _employeeRepository.GetCreditExchangeById(exchangeId);
        }
        public int RedeemCredits(string accountId, int pointsToRedeem)
        {
            var staff = _employeeRepository.GetStaffByAccountId(accountId);

            if (staff == null || staff.CreditPoints < pointsToRedeem)
            {
                return 0; // Return 0 for failure
            }

            // Deduct credits
            staff.CreditPoints -= pointsToRedeem;

            // Calculate cash equivalent and save in the CreditExchange table
            decimal cashEquivalent = pointsToRedeem * 0.5m;
            var creditExchange = new CreditExchange
            {
                AccountId = accountId,
                CreditPointsUsed = pointsToRedeem,
                CashAmount = cashEquivalent,
                ExchangeDate = DateTime.Now,
                Status = "Pending"
            };

            _employeeRepository.UpdateStaff(staff);
            _employeeRepository.RecordCreditExchange(creditExchange);

            return creditExchange.ExchangeId; // Return the exchange_id
        }

        public (bool Success, int ExchangeId, string Message, decimal CashEquivalent) SubmitExchange(string accountId, int pointsToRedeem, int availableCredits)
        {
            if (string.IsNullOrEmpty(accountId))
            {
                return (false, 0, "Account ID is invalid.", 0);
            }

            // Kiểm tra nếu số điểm yêu cầu quy đổi không hợp lệ
            if (pointsToRedeem < 100)
            {
                return (false, 0, "Minimum 100 reward points redeem at a time.", availableCredits * 0.5m);
            }
            if (pointsToRedeem > availableCredits)
            {
                return (false, 0, "Insufficient reward points.", availableCredits * 0.5m);
            }

            // Gọi RedeemCredits để thực hiện quy đổi
            var exchangeId = RedeemCredits(accountId, pointsToRedeem);
            Console.WriteLine($"RedeemCredits returned exchangeId: {exchangeId}");



            // Nếu exchangeId trả về là 0, điều đó có nghĩa là quy đổi thất bại
            if (exchangeId == 0)
            {
                return (false, 0, "Failed to redeem credits. Please try again later.", availableCredits * 0.5m);
            }

            // Nếu thành công, trả về kết quả với Success = true
            return (true, exchangeId, null, availableCredits * 0.5m);
        }

        public (bool Success, CreditExchange Exchange, int AvailableCredits, decimal CashEquivalent) GetExchangeConfirmation(int exchangeId)
        {
            var exchange = GetCreditExchangeById(exchangeId);

            if (exchange == null || exchange.Account == null)
            {
                return (false, null, 0, 0);
            }

            int availableCredits = exchange.Account.CreditPoints ?? 0;
            decimal cashEquivalent = exchange.CashAmount;

            return (true, exchange, availableCredits, cashEquivalent);
        }


        public Employee GetEmployeeByAccountId(string accountId)
        {
            return _employeeRepository.GetEmployeeByAccountId(accountId);
        }
        public List<Subtask> GetAssignedSubtasks(string accountId)
        {
            return _employeeRepository.GetAssignedSubtasks(accountId);
        }
        public List<PersonalTask> GetPersonalTasks(string accountId)
        {
            return _employeeRepository.GetPersonalTasks(accountId);
        }
        public (int BeforeDeadline, int OnDeadline, int AfterDeadline) GetCompletedTaskStats(string accountId)
        {
            var completedTasks = _employeeRepository.GetAssignedSubtasks(accountId)
                    .Where(s => s.Status == "Completed" && s.SubmissionDate.HasValue && s.Deadline.HasValue)
                    .ToList();


            int beforeDeadline = completedTasks.Count(s => s.SubmissionDate < s.Deadline);
            int onDeadline = completedTasks.Count(s => s.SubmissionDate == s.Deadline);
            int afterDeadline = completedTasks.Count(s => s.SubmissionDate > s.Deadline);

            return (beforeDeadline, onDeadline, afterDeadline);
        }

        public (int Completed, int Canceled, int Incomplete) GetTaskStatusCounts(string accountId)
        {
            var tasks = _employeeRepository.GetAssignedSubtasks(accountId);

            int completedCount = tasks.Count(s => s.Status == "Completed");
            int canceledCount = tasks.Count(s => s.Status == "Canceled");
            int incompleteCount = tasks.Count(s => s.Status != "Completed" && s.Status != "Canceled");

            return (completedCount, canceledCount, incompleteCount);
        }
        public Dictionary<int, int> GetTaskDifficultyStats(string accountId)
        {
            return _employeeRepository.GetTaskDifficultyStats(accountId);
        }
        public StaffAndLeader GetKPIData(string accountId)
        {
            return _employeeRepository.GetKPIData(accountId);
        }
        public bool UpdateEmployeeProfile(string accountId, Employee updatedEmployee)
        {
            return _employeeRepository.UpdateEmployee(accountId, updatedEmployee);
        }

        public StaffAndLeader GetStaffByAccountId(string accountId)
        {
            return _employeeRepository.GetStaffByAccountId(accountId); // Make sure to implement this method
        }
        
        public string GetTeamIdByAccountId(string accountId)
        {
            return _employeeRepository.GetTeamIdByAccountId(accountId);
        }
        public string GetDepartmentHeadBySubtaskId(string subtaskId)
        {
            return _employeeRepository.GetDepartmentHeadBySubtaskId(subtaskId);
        }
        public List<Employee> GetStaffByTeamId(string teamId)
        {
            // Gọi phương thức trong Repository để lấy danh sách nhân viên cùng teamId
            return _employeeRepository.GetStaffByTeamId(teamId);
        }
        public IPagedList<Employee> GetStaffByTeamIdWithFilters(string teamId, string accountId, string fullname, int pageNumber, int pageSize)
        {
            // Lấy danh sách nhân viên từ Repository
            var staffs = _employeeRepository.GetStaffByTeamId(teamId).AsQueryable();

            // Lọc theo accountId nếu có
            if (!string.IsNullOrEmpty(accountId))
            {
                staffs = staffs.Where(s => s.AccountId.Contains(accountId));
            }

            // Lọc theo fullname nếu có
            if (!string.IsNullOrEmpty(fullname))
            {
                string lowerFullname = fullname.ToLower();
                staffs = staffs.Where(s => s.Fullname.ToLower().Contains(lowerFullname));
            }

            // Áp dụng phân trang
            return staffs.ToPagedList(pageNumber, pageSize);
        }
        public List<Comment> GetCommentsBySubtaskId(string subtaskId)
        {
            return _employeeRepository.GetCommentsBySubtaskId(subtaskId);
        }
        public void AddComment(string accountId, string subtaskId, string content)
        {
            // Generate a new comment ID based on the existing comments in the database
            string lastCommentId = _employeeRepository.GetLastCommentId();
            int nextId = 1;

            if (!string.IsNullOrEmpty(lastCommentId) && lastCommentId.Length > 3)
            {
                int currentId = int.Parse(lastCommentId.Substring(3));
                nextId = currentId + 1;
            }

            string newCommentId = $"CMT{nextId:D3}";

            var comment = new Comment
            {
                CommentId = newCommentId,
                SubtaskId = subtaskId,
                Content = content,
                DateSubmitted = DateTime.Now
            };

            _employeeRepository.AddComment(comment);
        }

        public void DeleteComment(string commentId)
        {
            _employeeRepository.DeleteComment(commentId);
        }

        public IPagedList<Employee> GetTeamMembers(string teamId, string status, string role, string gender, DateTime? dobMin, DateTime? dobMax,
                                               DateTime? startDateMin, DateTime? startDateMax, DateTime? endDateMin, DateTime? endDateMax,
                                               int pageNumber, int pageSize, string memberType)
        {
            var filteredMembers = _employeeRepository.GetFilteredMembers(teamId, status, role, gender, dobMin, dobMax, startDateMin,
                                                                         startDateMax, endDateMin, endDateMax);

            var nonTeamMembers = _employeeRepository.GetNonTeamMembers(teamId, status, role, gender, dobMin, dobMax, startDateMin,
                                                                       startDateMax, endDateMin, endDateMax);

            // Chuyển đổi danh sách thành IPagedList
            return memberType == "notInTeam"
                ? nonTeamMembers.ToPagedList(pageNumber, pageSize) : filteredMembers.ToPagedList(pageNumber, pageSize);
        }

    }
}
