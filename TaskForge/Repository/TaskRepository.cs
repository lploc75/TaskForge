using System;
using System.Collections.Generic;
using System.Linq;
using TaskForge.Models;
using TaskForge.DBContext;

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

        // Gán task cho các phòng ban
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
        public List<TaskForge.Models.Task> GetTasksByDepartmentId(string deptId)
        {
            return _context.DepartmentTasks
                           .Where(dt => dt.DeptId == deptId)
                           .Select(dt => dt.Task)  // Lấy task từ DepartmentTask
                           .ToList();
        }


    }
}