// EmployeeRepository.cs
using TaskForge.Models;
using TaskForge.DBContext;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using static TaskForge.Repository.EmployeeRepository;

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

        public void RecordCreditExchange(CreditExchange creditExchange)
        {
            _context.CreditExchanges.Add(creditExchange);
            _context.SaveChanges();
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

        public List<Employee> GetStaffByTeam(string teamId)
        {
            return _context.Set<Employee>()
                           .Where(e => e.Teams.Any(t => t.TeamId == teamId) && e.Role == "Staff")
                           .ToList();
        }
        // Phương thức lấy team_id dựa trên account_id của nhân viên
        public string GetTeamIdByAccountId(string accountId)
        {
            var sql = @"SELECT TOP 1 t.team_id 
                    FROM Employee e 
                    JOIN EmployeeTeam et ON e.account_id = et.account_id
                    JOIN Team t ON et.team_id = t.team_id
                    WHERE e.account_id = @accountId";

            var teamId = _context.Set<Team>()
                                 .FromSqlRaw(sql, new SqlParameter("@accountId", accountId))
                                 .Select(t => t.TeamId)
                                 .FirstOrDefault();

            return teamId;
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

        // Phương thức lấy danh sách nhân viên có cùng teamId bằng SQL
        public List<Employee> GetStaffByTeamId(string teamId)
        {
            var sql = @"SELECT e.* 
                    FROM Employee e
                    JOIN EmployeeTeam et ON e.account_id = et.account_id
                    WHERE et.team_id = @teamId AND e.role = 'Staff'";

            return _context.Employees
                           .FromSqlRaw(sql, new SqlParameter("@teamId", teamId))
                           .ToList();
        }
        public List<Comment> GetCommentsBySubtaskId(string subtaskId)
        {
            var comments = _context.Comments
                .Include(c => c.Subtask)
                .ThenInclude(s => s.SubtaskAssignments)
                .ThenInclude(sa => sa.AssignedToNavigation) // Ensures navigation to Employee details
                .Where(c => c.SubtaskId == subtaskId)
                .Select(c => new Comment
                {
                    CommentId = c.CommentId,
                    Content = c.Content,
                    DateSubmitted = c.DateSubmitted,
                    SubtaskId = c.SubtaskId,
                    Subtask = new Subtask
                    {
                        SubtaskAssignments = new List<SubtaskAssignment>
                        {
                    new SubtaskAssignment
                    {
                        AssignedToNavigation = new Employee
                        {
                            Fullname = c.Subtask.SubtaskAssignments.FirstOrDefault() != null
                                       ? c.Subtask.SubtaskAssignments.FirstOrDefault().AssignedToNavigation.Fullname
                                       : "N/A",
                            AccountId = c.Subtask.SubtaskAssignments.FirstOrDefault() != null
                                        ? c.Subtask.SubtaskAssignments.FirstOrDefault().AssignedToNavigation.AccountId
                                        : "N/A"
                        }
                    }
                        }
                    }
                })
                .ToList();

            return comments;
        }


        public void AddComment(Comment comment)
        {
            _context.Comments.Add(comment);
            _context.SaveChanges();
        }

        public string GetLastCommentId()
        {
            return _context.Comments
                           .OrderByDescending(c => c.CommentId)
                           .Select(c => c.CommentId)
                           .FirstOrDefault();
        }

        public void DeleteComment(string commentId)
        {
            var comment = _context.Comments.FirstOrDefault(c => c.CommentId == commentId);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                _context.SaveChanges();
            }
        }

        public List<Employee> GetFilteredMembers(string teamId, string status, string role, string gender, DateTime? dobMin, DateTime? dobMax,
                                         DateTime? startDateMin, DateTime? startDateMax, DateTime? endDateMin,
                                         DateTime? endDateMax)
        {
            string sql = @"
    SELECT e.* 
    FROM Employee e
    LEFT JOIN EmployeeTeam et ON e.account_id = et.account_id
    WHERE et.team_id = @teamId
    AND e.role IN ('Staff', 'Leader')";

            var parameters = new List<SqlParameter>
    {
        new SqlParameter("@teamId", teamId)
    };

            if (!string.IsNullOrEmpty(status))
            {
                sql += " AND e.status = @status";
                parameters.Add(new SqlParameter("@status", status));
            }

            if (!string.IsNullOrEmpty(role))
            {
                sql += " AND e.role = @role";
                parameters.Add(new SqlParameter("@role", role));
            }

            if (!string.IsNullOrEmpty(gender))
            {
                sql += " AND e.gender = @gender";
                parameters.Add(new SqlParameter("@gender", gender));
            }

            if (dobMin.HasValue)
            {
                sql += " AND e.dob >= @dobMin";
                parameters.Add(new SqlParameter("@dobMin", dobMin.Value));
            }

            if (dobMax.HasValue)
            {
                sql += " AND e.dob <= @dobMax";
                parameters.Add(new SqlParameter("@dobMax", dobMax.Value));
            }

            if (startDateMin.HasValue)
            {
                sql += " AND e.start_date >= @startDateMin";
                parameters.Add(new SqlParameter("@startDateMin", startDateMin.Value));
            }

            if (startDateMax.HasValue)
            {
                sql += " AND e.start_date <= @startDateMax";
                parameters.Add(new SqlParameter("@startDateMax", startDateMax.Value));
            }

            if (endDateMin.HasValue)
            {
                sql += " AND e.end_date >= @endDateMin";
                parameters.Add(new SqlParameter("@endDateMin", endDateMin.Value));
            }

            if (endDateMax.HasValue)
            {
                sql += " AND e.end_date <= @endDateMax";
                parameters.Add(new SqlParameter("@endDateMax", endDateMax.Value));
            }

            return _context.Employees.FromSqlRaw(sql, parameters.ToArray()).ToList();
        }


        public List<Employee> GetNonTeamMembers(string teamId, string status, string role, string gender, DateTime? dobMin, DateTime? dobMax,
                                                DateTime? startDateMin, DateTime? startDateMax, DateTime? endDateMin, DateTime? endDateMax)
        {
            string sql = @"
SELECT e.* 
FROM Employee e
LEFT JOIN EmployeeTeam et ON e.account_id = et.account_id
WHERE (et.team_id != @teamId OR et.team_id IS NULL)
AND e.role IN ('Staff', 'Leader')";

            var parameters = new List<SqlParameter>
        {
            new SqlParameter("@teamId", teamId)
        };

            if (!string.IsNullOrEmpty(status))
            {
                sql += " AND e.status = @status";
                parameters.Add(new SqlParameter("@status", status));
            }

            if (!string.IsNullOrEmpty(role))
            {
                sql += " AND e.role = @role";
                parameters.Add(new SqlParameter("@role", role));
            }

            if (!string.IsNullOrEmpty(gender))
            {
                sql += " AND e.gender = @gender";
                parameters.Add(new SqlParameter("@gender", gender));
            }

            if (dobMin.HasValue)
            {
                sql += " AND e.dob >= @dobMin";
                parameters.Add(new SqlParameter("@dobMin", dobMin.Value));
            }

            if (dobMax.HasValue)
            {
                sql += " AND e.dob <= @dobMax";
                parameters.Add(new SqlParameter("@dobMax", dobMax.Value));
            }

            if (startDateMin.HasValue)
            {
                sql += " AND e.start_date >= @startDateMin";
                parameters.Add(new SqlParameter("@startDateMin", startDateMin.Value));
            }

            if (startDateMax.HasValue)
            {
                sql += " AND e.start_date <= @startDateMax";
                parameters.Add(new SqlParameter("@startDateMax", startDateMax.Value));
            }

            if (endDateMin.HasValue)
            {
                sql += " AND e.end_date >= @endDateMin";
                parameters.Add(new SqlParameter("@endDateMin", endDateMin.Value));
            }

            if (endDateMax.HasValue)
            {
                sql += " AND e.end_date <= @endDateMax";
                parameters.Add(new SqlParameter("@endDateMax", endDateMax.Value));
            }

            return _context.Employees.FromSqlRaw(sql, parameters.ToArray()).ToList();
        }
    }

}

