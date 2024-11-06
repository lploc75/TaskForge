using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TaskForge.DBContext;
using TaskForge.Models;

namespace TaskForge.Repository
{
    public class SubtaskRepository
    {
        private readonly TaskForgeContext _context;

        public SubtaskRepository(TaskForgeContext context)
        {
            _context = context;
        }

        public List<Subtask> GetSubtasksByTaskId(string taskId)
        {
            return _context.Subtasks
                           .Include(st => st.Task) // Bao gồm Task để chắc chắn không có vấn đề với liên kết
                           .Include(st => st.Team) // Bao gồm Team của từng Subtask
                           .Where(st => st.TaskId == taskId)
                           .ToList();
        }

        public Subtask GetSubtaskById(string id)
        {
            string sql = "SELECT * FROM Subtask WHERE subtask_id = @id";
            var parameter = new SqlParameter("@id", id);
            return _context.Subtasks.FromSqlRaw(sql, parameter).FirstOrDefault();
        }
        public void CreateSubtask(Subtask subtask)
        {
            if (string.IsNullOrEmpty(subtask.SubtaskId))
            {
                subtask.SubtaskId = Guid.NewGuid().ToString().Substring(0, 10); // Tạo SubtaskId ngẫu nhiên
            }

            subtask.AssignmentDate = DateTime.Now; // Gán ngày giao việc hiện tại

            _context.Subtasks.Add(subtask); // Thêm subtask vào DB
            _context.SaveChanges(); // Lưu vào DB
        }

        public void UpdateSubtask(Subtask subtask)
        {
            var sql = @"
        UPDATE Subtask
        SET 
            subtask_name = {0}, 
            description = {1}, 
            priority = {2}, 
            deadline = {3}, 
            status = {4}
        WHERE 
            subtask_id = {5}";

            _context.Database.ExecuteSqlRaw(
                sql,
                subtask.SubtaskName,
                subtask.Description,
                subtask.Priority,
                subtask.Deadline,
                subtask.Status,
                subtask.SubtaskId
            );
        }


        public void DeleteSubtask(string subtaskId)
        {
            var subtask = _context.Subtasks.FirstOrDefault(st => st.SubtaskId == subtaskId);

            if (subtask != null)
            {
                _context.Subtasks.Remove(subtask);
                _context.SaveChanges();
            }
        }
        public List<Employee> GetEmployeesBySubtaskId(string subtaskId)
        {
            return _context.SubtaskAssignments
                           .Where(sa => sa.SubtaskId == subtaskId)
                           .Select(sa => sa.AssignedToNavigation)
                           .ToList();
        }
        public void SaveEvaluation(SubtaskEvaluation evaluation)
        {
            // Tìm EvaluationId cuối cùng trong cơ sở dữ liệu và tăng lên 1
            var lastEvaluationId = _context.SubtaskEvaluations
                                           .OrderByDescending(e => e.EvaluationId)
                                           .Select(e => e.EvaluationId)
                                           .FirstOrDefault();

            // Nếu không có EvaluationId, bắt đầu từ "SEVAL001"
            int nextId = lastEvaluationId != null ? int.Parse(lastEvaluationId.Substring(5)) + 1 : 1;

            // Format EvaluationId thành "SEVAL001", "SEVAL002", ...
            evaluation.EvaluationId = "SEVAL" + nextId.ToString("D3");

            // Thêm đánh giá vào cơ sở dữ liệu
            _context.SubtaskEvaluations.Add(evaluation);
            _context.SaveChanges();
        }

        public List<Subtask> FilterSubtasks(string status, int? priority, int? difficulty, DateTime? assignmentDateMin, DateTime? assignmentDateMax, DateTime? deadlineMin, DateTime? deadlineMax, string submission, string taskId, string teamId)
        {
            string sql = "SELECT * FROM Subtask WHERE 1=1";
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(status))
            {
                sql += " AND status = @status";
                parameters.Add(new SqlParameter("@status", status));
            }
            if (priority.HasValue)
            {
                sql += " AND priority = @priority";
                parameters.Add(new SqlParameter("@priority", priority));
            }
            if (difficulty.HasValue)
            {
                sql += " AND difficulty = @difficulty";
                parameters.Add(new SqlParameter("@difficulty", difficulty));
            }
            if (assignmentDateMin.HasValue)
            {
                sql += " AND assignment_date >= @assignmentDateMin";
                parameters.Add(new SqlParameter("@assignmentDateMin", assignmentDateMin));
            }
            if (assignmentDateMax.HasValue)
            {
                sql += " AND assignment_date <= @assignmentDateMax";
                parameters.Add(new SqlParameter("@assignmentDateMax", assignmentDateMax));
            }
            if (deadlineMin.HasValue)
            {
                sql += " AND deadline >= @deadlineMin";
                parameters.Add(new SqlParameter("@deadlineMin", deadlineMin));
            }
            if (deadlineMax.HasValue)
            {
                sql += " AND deadline <= @deadlineMax";
                parameters.Add(new SqlParameter("@deadlineMax", deadlineMax));
            }
            if (!string.IsNullOrEmpty(submission))
            {
                if (submission == "Yes")
                {
                    sql += " AND submission_date IS NOT NULL";
                }
                else if (submission == "No")
                {
                    sql += " AND submission_date IS NULL";
                }
            }
            if (!string.IsNullOrEmpty(taskId))
            {
                sql += " AND task_id LIKE @taskId";
                parameters.Add(new SqlParameter("@taskId", $"%{taskId}%"));
            }
            if (!string.IsNullOrEmpty(teamId))
            {
                sql += " AND team_id LIKE @teamId";
                parameters.Add(new SqlParameter("@teamId", $"%{teamId}%"));
            }

            return _context.Subtasks.FromSqlRaw(sql, parameters.ToArray()).ToList();
        }

        public void RemoveSubtaskAssignment(string subtaskId)
        {
            // Tìm và xóa bản ghi SubtaskAssignment dựa trên subtaskId
            _context.Database.ExecuteSqlRaw("DELETE FROM SubtaskAssignment WHERE subtask_id = {0}", subtaskId);

            // Tìm và cập nhật trạng thái của Subtask thành "Not Assign"
            _context.Database.ExecuteSqlRaw("UPDATE Subtask SET status = 'Not Assign' WHERE subtask_id = {0}", subtaskId);
        }

    }
}