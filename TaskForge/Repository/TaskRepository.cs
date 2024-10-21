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


    }
}
