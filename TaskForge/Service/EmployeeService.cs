using TaskForge.Models;
using TaskForge.Repository;

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

        public bool UpdateSubtaskStatus(string subtaskId, string status)
        {
            // Gọi repository để lấy subtask
            var subtask = _employeeRepository.GetSubtaskById(subtaskId);
            if (subtask != null)
            {
                subtask.Status = status;
                if (status == "Pending")
                {
                    subtask.SubmissionDate = DateTime.Now; // Cập nhật SubmissionDate
                }
                else
                {
                    subtask.SubmissionDate = null;
                }
                _employeeRepository.UpdateSubtask(subtask);
                return true;
            }
            return false;
        }
        public bool UpdatePersonalTaskkStatus(string subtaskId, string status)
        {
            // Gọi repository để lấy subtask
            var Ptask = _employeeRepository.GetPtaskById(subtaskId);
            if (Ptask != null)
            {
                Ptask.Status = status;
                _employeeRepository.UpdatePtask(Ptask);
                return true;
            }
            return false;
        }
        public StaffAndLeader GetStaffByAccountId(string accountId)
        {
            return _employeeRepository.GetStaffByAccountId(accountId); // Make sure to implement this method
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
    }
}
