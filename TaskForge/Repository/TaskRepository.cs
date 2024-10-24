using System;
using System.Collections.Generic;
using System.Linq;
using TaskForge.Models;
using TaskForge.DBContext;
using Microsoft.EntityFrameworkCore;

namespace TaskForge.Repository
{
    public class TaskRepository
    {
        private readonly TaskForgeContext _context;

        public TaskRepository(TaskForgeContext context)
        {
            _context = context;
        }

        // Lấy tất cả các task theo ProjectId
        public List<TaskForge.Models.Task> GetTasksByProjectId(int projectId)
        {
            return _context.Tasks
                .Where(t => t.ProjectId == projectId)
                .ToList();
        }
        // Lấy Task theo Id
        public TaskForge.Models.Task GetTaskById(string taskId)
        {
            return _context.Tasks.FirstOrDefault(t => t.TaskId == taskId);
        }
        // Cập nhật Task
        public void UpdateTask(TaskForge.Models.Task task)
        {
            var existingTask = _context.Tasks.FirstOrDefault(t => t.TaskId == task.TaskId);
            if (existingTask != null)
            {
                // Cập nhật thông tin task
                existingTask.TaskName = task.TaskName;
                existingTask.Description = task.Description;
                existingTask.Priority = task.Priority;
                existingTask.AssignmentDate = task.AssignmentDate;
                existingTask.Deadline = task.Deadline;
                existingTask.Status = task.Status;

                _context.Tasks.Update(existingTask);
                _context.SaveChanges();
            }
            else
            {
                throw new KeyNotFoundException("Task not found");
            }
        }

        // Xóa Task
        public void DeleteTask(string taskId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Xóa các bản ghi liên quan trong bảng TaskAssignment
                    var taskAssignments = _context.TaskAssignments.Where(ta => ta.TaskId == taskId).ToList();
                    if (taskAssignments.Any())
                    {
                        _context.TaskAssignments.RemoveRange(taskAssignments);
                    }

                    // Xóa các bản ghi liên quan trong bảng TaskEvaluation
                    var taskEvaluations = _context.TaskEvaluations.Where(te => te.TaskId == taskId).ToList();
                    if (taskEvaluations.Any())
                    {
                        _context.TaskEvaluations.RemoveRange(taskEvaluations);
                    }

                    // Xóa các bản ghi liên quan trong bảng DepartmentTask
                    var departmentTasks = _context.DepartmentTasks.Where(dt => dt.TaskId == taskId).ToList();
                    if (departmentTasks.Any())
                    {
                        _context.DepartmentTasks.RemoveRange(departmentTasks);
                    }

                    // Xóa tất cả các subtasks và các liên kết của chúng
                    var subtasks = _context.Subtasks.Where(st => st.TaskId == taskId).ToList();
                    foreach (var subtask in subtasks)
                    {
                        // Xóa tất cả các bản ghi liên quan trong bảng SubtaskAssignment
                        var subtaskAssignments = _context.SubtaskAssignments.Where(sa => sa.SubtaskId == subtask.SubtaskId).ToList();
                        if (subtaskAssignments.Any())
                        {
                            _context.SubtaskAssignments.RemoveRange(subtaskAssignments);
                        }

                        // Xóa tất cả các bản ghi liên quan trong bảng SubtaskEvaluation
                        var subtaskEvaluations = _context.SubtaskEvaluations.Where(se => se.SubtaskId == subtask.SubtaskId).ToList();
                        if (subtaskEvaluations.Any())
                        {
                            _context.SubtaskEvaluations.RemoveRange(subtaskEvaluations);
                        }

                        // Xóa các bình luận liên quan đến subtask
                        var comments = _context.Comments.Where(c => c.SubtaskId == subtask.SubtaskId).ToList();
                        if (comments.Any())
                        {
                            _context.Comments.RemoveRange(comments);
                        }

                        // Cuối cùng, xóa subtask
                        _context.Subtasks.Remove(subtask);
                    }

                    // Sau khi đã xóa hết các liên kết, xóa task
                    var task = _context.Tasks.FirstOrDefault(t => t.TaskId == taskId);
                    if (task != null)
                    {
                        _context.Tasks.Remove(task);
                    }

                    // Lưu các thay đổi vào database
                    _context.SaveChanges();

                    // Commit transaction nếu không có lỗi xảy ra
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    // Rollback transaction nếu có lỗi
                    transaction.Rollback();
                    throw new Exception("Error deleting task: " + ex.Message);
                }
            }
        }

        // Thêm một task mới vào dự án
        public void CreateTask(TaskForge.Models.Task task, List<string> departmentIds)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (string.IsNullOrEmpty(task.TaskId))
                    {
                        task.TaskId = Guid.NewGuid().ToString().Substring(0, 10);
                    }

                    _context.Tasks.Add(task);
                    _context.SaveChanges();

                    AssignTaskToDepartments(task.TaskId, departmentIds);

                    transaction.Commit();  // Xác nhận transaction nếu không có lỗi
                }
                catch (Exception ex)
                {
                    transaction.Rollback();  // Rollback nếu có lỗi xảy ra
                    throw;  // Hoặc xử lý lỗi tùy ý
                }
            }
        }
        // Lấy Personal Task theo Id
        public PersonalTask GetPersonalTaskById(string PtaskId)
        {
            return _context.PersonalTasks.Find(PtaskId);
        }

        // Cập nhật Personal Task
        public void UpdatePersonalTask(PersonalTask personalTask)
        {
            _context.PersonalTasks.Update(personalTask);
            _context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
        }
        public PersonalTask GetPersonalTasksById(string ptaskId)
        {
            return _context.PersonalTasks.FirstOrDefault(t => t.PtaskId == ptaskId);
        }

        public void DeletePersonalTasks(PersonalTask task)
        {
            _context.PersonalTasks.Remove(task);
            _context.SaveChanges();

        }

        public void AssignTaskToDepartments(string taskId, List<string> departmentIds)
        {
            foreach (var departmentId in departmentIds)
            {
                // Kiểm tra xem department có tồn tại không
                var department = _context.Departments.FirstOrDefault(d => d.DeptId == departmentId);
                if (department != null)
                {
                    var departmentTask = new DepartmentTask
                    {
                        TaskId = taskId,
                        DeptId = departmentId
                    };

                    _context.DepartmentTasks.Add(departmentTask);
                }
                else
                {
                    // Ném ngoại lệ nếu không tìm thấy department
                    throw new Exception($"Department with ID {departmentId} not found.");
                }
            }

            try
            {
                // Cố gắng lưu các thay đổi, nếu có lỗi, ném ngoại lệ để hiển thị ra màn hình
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to assign task {taskId} to departments. Error: {ex.Message}");
            }
        }

        // Thêm dự án mới vào database
        public void AddPersonalTask(PersonalTask ptask)
        {
            _context.PersonalTasks.Add(ptask);
            _context.SaveChanges();
        }
        // Lấy danh sách Subtask đã lọc từ cơ sở dữ liệu cho người dùng
        public async Task<List<Subtask>> GetFilteredSubtasksForUserAsync(string accountId, string status, string priority, string difficulty, DateTime? deadline)
        {
            // Join giữa Subtask và SubtaskAssignment để lấy các subtask được giao cho người dùng cụ thể
            var query = from subtask in _context.Subtasks
                        join assignment in _context.SubtaskAssignments
                        on subtask.SubtaskId equals assignment.SubtaskId
                        where assignment.AssignedTo == accountId
                        select subtask;

            // Các điều kiện lọc
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(t => t.Status == status);
            }

            if (!string.IsNullOrEmpty(priority))
            {
                query = query.Where(t => t.Priority == int.Parse(priority));
            }

            if (!string.IsNullOrEmpty(difficulty))
            {
                query = query.Where(t => t.Difficulty == int.Parse(difficulty));
            }

            if (deadline.HasValue)
            {
                query = query.Where(t => t.Deadline <= deadline);
            }

            return await query.ToListAsync();
        }


        // Lấy danh sách PersonalTask đã lọc từ cơ sở dữ liệu cho người dùng
        public async Task<List<PersonalTask>> GetFilteredPersonalTasksForUserAsync(string accountId, string status, string priority, DateTime? deadline)
        {
            var query = _context.PersonalTasks.Where(t => t.AccountId == accountId).AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(t => t.Status == status);
            }

            if (!string.IsNullOrEmpty(priority))
            {
                query = query.Where(t => t.Priority == int.Parse(priority));
            }

            if (deadline.HasValue)
            {
                query = query.Where(t => t.Deadline <= deadline);
            }

            return await query.ToListAsync();
        }

    }
}