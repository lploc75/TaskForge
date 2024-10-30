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

        public Subtask GetSubtaskById(string subtaskId)
        {
            return _context.Subtasks
                           .FirstOrDefault(st => st.SubtaskId == subtaskId);
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
            var existingSubtask = _context.Subtasks.FirstOrDefault(st => st.SubtaskId == subtask.SubtaskId);
            if (existingSubtask != null)
            {
                existingSubtask.SubtaskName = subtask.SubtaskName;
                existingSubtask.Description = subtask.Description;
                existingSubtask.Priority = subtask.Priority;
                existingSubtask.Deadline = subtask.Deadline;
                existingSubtask.Status = subtask.Status;

                _context.Subtasks.Update(existingSubtask);
                _context.SaveChanges();
            }
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



    }
}