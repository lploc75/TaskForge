using System;
using System.Collections.Generic;
using System.Linq;
using TaskForge.Models;
using TaskForge.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;

namespace TaskForge.Repository
{
    public class TaskRepository
    {
        private readonly TaskForgeContext _context;

        public TaskRepository(TaskForgeContext context)
        {
            _context = context;
        }

        //// Lấy tất cả các task theo ProjectId
        //public List<TaskForge.Models.Task> GetTasksByProjectId(int projectId)
        //{
        //    return _context.Tasks
        //        .Where(t => t.ProjectId == projectId)
        //        .ToList();
        //}
        //// Lấy Task theo Id
        //public TaskForge.Models.Task GetTaskById(string taskId)
        //{
        //    return _context.Tasks.FirstOrDefault(t => t.TaskId == taskId);
        //}

        // Lấy tất cả các task theo ProjectId
        public List<TaskForge.Models.Task> GetTasksByProjectId(int projectId)
        {
            // Bao gồm cả bảng trung gian DepartmentTask và Department khi truy vấn các task
            return _context.Tasks
                .Where(t => t.ProjectId == projectId)
                .Include(t => t.DepartmentTasks) // Bao gồm bảng trung gian
                    .ThenInclude(dt => dt.Dept)  // Bao gồm bảng Department từ bảng trung gian
                .ToList();
        }
        // Lấy Task theo Id, bao gồm thông tin phòng ban
        public TaskForge.Models.Task GetTaskById(string taskId)
        {
            return _context.Tasks
                .Include(t => t.DepartmentTasks)  // Bao gồm bảng DepartmentTask
                    .ThenInclude(dt => dt.Dept)   // Bao gồm bảng Department
                .FirstOrDefault(t => t.TaskId == taskId);
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

        // Truy xuất PersonalTask bằng SQL raw dựa trên PtaskId
        public PersonalTask GetPersonalTaskById(string id)
        {
            string sql = "SELECT * FROM PersonalTask WHERE ptask_id = @id";
            var parameter = new SqlParameter("@id", id);
            return _context.PersonalTasks.FromSqlRaw(sql, parameter).FirstOrDefault();
        }

        // Cập nhật thông tin của Personal Task với SQL raw
        public void UpdatePersonalTask(PersonalTask personalTask)
        {
            string sql = @"
        UPDATE PersonalTask 
        SET 
            ptask_name = @ptask_name, 
            description = @description, 
            assignment_date = @assignment_date, 
            deadline = @deadline, 
            priority = @priority 
        WHERE ptask_id = @ptask_id";

            _context.Database.ExecuteSqlRaw(sql,
                new SqlParameter("@ptask_name", personalTask.PtaskName),
                new SqlParameter("@description", personalTask.Description),
                new SqlParameter("@assignment_date", personalTask.AssignmentDate),
                new SqlParameter("@deadline", personalTask.Deadline),
                new SqlParameter("@priority", personalTask.Priority),
                new SqlParameter("@ptask_id", personalTask.PtaskId)
            );
        }

        public void DeletePersonalTask(string ptaskId)
        {
            string sql = "DELETE FROM PersonalTask WHERE ptask_id = @ptask_id";
            _context.Database.ExecuteSqlRaw(sql, new SqlParameter("@ptask_id", ptaskId));
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

        // Thêm dự án mới vào database
        public void AddPersonalTask(PersonalTask ptask)
        {
          // Thêm PersonalTask mới vào database bằng SQL raw
         string sql = @"
        INSERT INTO PersonalTask (ptask_id, account_id, ptask_name, status, priority, assignment_date, deadline, description) 
        VALUES (@ptask_id, @account_id, @ptask_name, @status, @priority, @assignment_date, @deadline, @description)";

            _context.Database.ExecuteSqlRaw(sql,
                new SqlParameter("@ptask_id", ptask.PtaskId),
                new SqlParameter("@account_id", ptask.AccountId),
                new SqlParameter("@ptask_name", ptask.PtaskName),
                new SqlParameter("@status", ptask.Status),
                new SqlParameter("@priority", ptask.Priority),
                new SqlParameter("@assignment_date", ptask.AssignmentDate),
                new SqlParameter("@deadline", ptask.Deadline),
                new SqlParameter("@description", ptask.Description)
            );
        }
        public List<Subtask> GetAllSubtasks()
        {
            return _context.Subtasks.OrderByDescending(s => s.AssignmentDate).ToList();
        }
        public List<PersonalTask> GetAllPersonalTasks()
        {
            return _context.PersonalTasks.OrderByDescending(s => s.AssignmentDate).ToList();
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
        public List<Subtask> GetSubtasksByTeam(string teamId)
        {
            var sql = @"
        SELECT * 
        FROM Subtask 
        WHERE team_id = @teamId";

            var subtasks = _context.Subtasks
                                   .FromSqlRaw(sql, new SqlParameter("@teamId", teamId))
                                   .ToList();

            return subtasks;
        }
        public List<SubtaskAssignment> GetAllSubtaskAssignments()
        {
            var sql = "SELECT * FROM SubtaskAssignment";
            return _context.SubtaskAssignments.FromSqlRaw(sql).ToList();
        }
        public void AssignSubtask(string subtaskId, string assignedTo, string createdBy)
        {
            var subtask = _context.Set<Subtask>().Find(subtaskId);

            if (subtask != null && subtask.Status == "Not Assign")
            {
                var assignment = new SubtaskAssignment
                {
                    SubtaskId = subtaskId,
                    CreatedBy = createdBy,
                    AssignedTo = assignedTo
                };
                _context.Set<SubtaskAssignment>().Add(assignment);

                subtask.Status = "Not Start";
                _context.SaveChanges();
            }
        }

        public void UnassignSubtask(string subtaskId)
        {
            var assignment = _context.Set<SubtaskAssignment>()
                                     .FirstOrDefault(sa => sa.SubtaskId == subtaskId);

            if (assignment != null)
            {
                _context.Set<SubtaskAssignment>().Remove(assignment);

                var subtask = _context.Set<Subtask>().Find(subtaskId);
                if (subtask != null)
                {
                    subtask.Status = "Not Assign";
                }

                _context.SaveChanges();
            }
        }
        public List<PersonalTask> FilterPersonalTasks(string status, int? priority, DateTime? assignmentDateMin, DateTime? assignmentDateMax, DateTime? deadlineMin, DateTime? deadlineMax)
        {
            string sql = "SELECT * FROM PersonalTask WHERE 1=1";
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

            return _context.PersonalTasks.FromSqlRaw(sql, parameters.ToArray()).ToList();
        }

        public void UpdatePtask(PersonalTask Ptask)
        {
            _context.Database.ExecuteSqlRaw(
                "UPDATE PersonalTask SET ptask_name = {0}, description = {1}, status = {2}, deadline = {3} WHERE ptask_id = {4}",
                Ptask.PtaskName, Ptask.Description, Ptask.Status, Ptask.Deadline, Ptask.PtaskId
            );
        }


    }
}