// EmployeeRepository.cs
using TaskForge.Models;
using TaskForge.DBContext;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace TaskForge.Repository
{
    public class EmployeeRepository
    {
        private readonly TaskForgeContext _context;

        public EmployeeRepository(TaskForgeContext context)
        {
            _context = context;
        }

        public Employee GetEmployeeByAccountId(string accountId)
        {
            return _context.Employees
                .Include(e => e.Account)          // Include Account details
                .Include(e => e.Dept)             // Include Department details
                .FirstOrDefault(e => e.AccountId == accountId);
        }

        // Lấy StaffAndLeader dựa trên AccountId
        public StaffAndLeader GetStaffByAccountId(string accountId)
        {
            return _context.StaffAndLeaders
                           .FromSqlRaw("SELECT * FROM StaffAndLeader WHERE account_id = {0}", accountId)
                           .FirstOrDefault();
        }

        // Tạo một CreditExchange mới
        public int RecordCreditExchange(CreditExchange creditExchange)
        {
            // Thực hiện lệnh INSERT
            _context.Database.ExecuteSqlRaw(
                "INSERT INTO CreditExchange (account_id, exchange_date, credit_points_used, cash_amount, status) VALUES ({0}, {1}, {2}, {3}, {4})",
                creditExchange.AccountId,
                creditExchange.ExchangeDate,
                creditExchange.CreditPointsUsed,
                creditExchange.CashAmount,
                creditExchange.Status
            );

            // Lấy giá trị ID vừa chèn bằng cách chỉ truy vấn cột exchange_id
            var exchangeId = _context.CreditExchanges
                                     .FromSqlRaw("SELECT TOP 1 exchange_id FROM CreditExchange ORDER BY exchange_id DESC")
                                     .Select(e => e.ExchangeId)
                                     .FirstOrDefault();
            Console.WriteLine("exchangeId: " + exchangeId);

            return exchangeId;
        }


        public void UpdateStaff(StaffAndLeader staff)
        {
            _context.StaffAndLeaders.Update(staff);
            _context.SaveChanges();
        }
        public CreditExchange GetCreditExchangeById(int exchangeId)
        {
            return _context.CreditExchanges
                           .FromSqlRaw("SELECT * FROM CreditExchange WHERE exchange_id = {0}", exchangeId)
                           .Include(e => e.Account) // Bao gồm đối tượng Account
                           .FirstOrDefault();
        }
        public List<Subtask> GetAssignedSubtasks(string accountId)
        {
            return _context.SubtaskAssignments
                .Where(sa => sa.AssignedTo == accountId)
                .Include(sa => sa.Subtask.Comments)                // Truy xuất Comments của Subtask
                .Include(sa => sa.Subtask.SubtaskEvaluations)      // Truy xuất SubtaskEvaluations của Subtask
                .Include(sa => sa.Subtask.Team)                    // Truy xuất Team của Subtask
                .Include(sa => sa.Subtask.Task)                    // Truy xuất Task của Subtask
                .Select(sa => sa.Subtask)                          // Chọn Subtask từ SubtaskAssignment
                .ToList() ?? new List<Subtask>();
        }
        public List<PersonalTask> GetPersonalTasks(string accountId)
        {
            return _context.PersonalTasks
                .Where(pt => pt.AccountId == accountId)  // Lọc theo accountId
                .Select(pt => new PersonalTask
                {
                    PtaskId = pt.PtaskId,                // Lấy PtaskId
                    AccountId = pt.AccountId,            // Lấy AccountId
                    PtaskName = pt.PtaskName,            // lấy PtaskName
                    Status = pt.Status,                  // Lấy Status
                    Priority = pt.Priority,              // Lấy Priority
                    AssignmentDate = pt.AssignmentDate,  // Lấy AssignmentDate
                    Deadline = pt.Deadline,              // Lấy Deadline
                    Description = pt.Description         // Lấy Description
                })
                .ToList();
        }

        public Dictionary<int, int> GetTaskDifficultyStats(string accountId)
        {
            return _context.SubtaskAssignments
                .Where(sa => sa.AssignedTo == accountId && sa.Subtask.Difficulty.HasValue)
                .GroupBy(sa => sa.Subtask.Difficulty.Value)
                .Select(g => new { Difficulty = g.Key, Count = g.Count() })
                .ToDictionary(g => g.Difficulty, g => g.Count);
        }

        public StaffAndLeader GetKPIData(string accountId)
        {
            return _context.StaffAndLeaders.FirstOrDefault(s => s.AccountId == accountId);
        }

        public bool UpdateEmployee(string accountId, Employee updatedEmployee)
        {
            // Tìm nhân viên theo AccountId, bao gồm cả đối tượng Account liên quan
            var employee = _context.Employees
                                   .Include(e => e.Account)
                                   .FirstOrDefault(e => e.AccountId == accountId);

            if (employee != null)
            {
                // Cập nhật các trường trong Employee nếu không null
                if (!string.IsNullOrEmpty(updatedEmployee.Fullname))
                {
                    employee.Fullname = updatedEmployee.Fullname;
                }

                if (!string.IsNullOrEmpty(updatedEmployee.Gender))
                {
                    employee.Gender = updatedEmployee.Gender;
                }

                if (updatedEmployee.Dob.HasValue)
                {
                    employee.Dob = updatedEmployee.Dob;
                }

                // Cập nhật các trường trong Account, nếu Account không null
                if (updatedEmployee.Account != null)
                {
                    if (!string.IsNullOrEmpty(updatedEmployee.Account.Email))
                    {
                        employee.Account.Email = updatedEmployee.Account.Email;
                    }

                    if (!string.IsNullOrEmpty(updatedEmployee.Account.PhoneNumber))
                    {
                        employee.Account.PhoneNumber = updatedEmployee.Account.PhoneNumber;
                    }
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public void UpdateSubtask(Subtask subtask)
        {
            _context.Subtasks.Update(subtask);
            _context.SaveChanges();
        }
        public void UpdatePtask(PersonalTask ptask)
        {
            _context.PersonalTasks.Update(ptask);
            _context.SaveChanges();
        }
        public Subtask GetSubtaskById(string id)
        {
            return _context.Subtasks.Find(id);
        }
        public PersonalTask GetPtaskById(string id)
        {
            return _context.PersonalTasks.Find(id);
        }

        public List<Employee> GetStaffByTeam(string teamId)
        {
            return _context.Set<Employee>()
                           .Where(e => e.Teams.Any(t => t.TeamId == teamId) && e.Role == "Staff")
                           .ToList();
        }
        // Phương thức lấy team_id dựa trên account_id của nhân viên
        public string GetTeamIdByAccountId(string accountId)
        {
            var team = _context.Set<Employee>()
                               .Where(e => e.AccountId == accountId)
                               .SelectMany(e => e.Teams)
                               .FirstOrDefault();

            return team?.TeamId; // Trả về team_id nếu tìm thấy, ngược lại là null
        }
        public string GetDepartmentHeadBySubtaskId(string subtaskId)
        {
            // Truy vấn để lấy thông tin của Department Head (created_by)
            var createdBy = (from s in _context.Subtasks
                             join dt in _context.DepartmentTasks on s.TaskId equals dt.TaskId
                             join e in _context.Employees on dt.DeptId equals e.DeptId
                             where s.SubtaskId == subtaskId && e.Role == "Department Head"
                             select e.AccountId).FirstOrDefault();

            return createdBy;
        }

        public List<Employee> GetStaffByTeamId(string teamId)
        {
            // Lấy danh sách nhân viên có cùng teamId và role là "Staff"
            return _context.Employees
                .Where(e => e.Teams.Any(t => t.TeamId == teamId) && e.Role == "Staff")
                .ToList();
        }
    }
}
