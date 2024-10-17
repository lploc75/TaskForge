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

        public void UpdateSubtask(Models.Subtask subtask)
        {
            _context.Subtasks.Update(subtask);
            _context.SaveChanges();
        }

        public Models.Subtask GetSubtaskById(string id)
        {
            return _context.Subtasks.Find(id); // Đảm bảo `_context.Subtasks` đúng với bảng `subtask`
        }

    }
}
