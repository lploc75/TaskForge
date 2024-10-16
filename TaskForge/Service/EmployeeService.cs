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

        public Employee GetEmployeeByAccountId(string accountId)
        {
            return _employeeRepository.GetEmployeeByAccountId(accountId);
        }
        public List<Subtask> GetAssignedSubtasks(string accountId)
        {
            return _employeeRepository.GetAssignedSubtasks(accountId);
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
    }
}
