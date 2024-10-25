using System.Collections.Generic;
using System.Linq;
using TaskForge.Models;
using TaskForge.DBContext;
using Microsoft.EntityFrameworkCore;

namespace TaskForge.Repository
{
    public class ProjectRepository
    {
        private readonly TaskForgeContext _context;

        public ProjectRepository(TaskForgeContext context)
        {
            _context = context;
        }

        // Lấy danh sách dự án mà nhân viên có vai trò là "Manager"
        public List<Project> GetProjectsByStatusAndAccount(string status, string accountId)
        {
            return _context.EmployeeProjects
                .Where(ep => ep.AccountId == accountId && ep.Project.Status == status)
                .Select(ep => ep.Project)
                .ToList();
        }

        // Lấy chi tiết dự án theo ID
        public Project GetProjectById(int projectId)
        {
            return _context.Projects
                .Include(p => p.Departments)  // Bao gồm các phòng ban liên kết với dự án
                .FirstOrDefault(p => p.ProjectId == projectId);
        }


        // Thêm dự án mới vào database
        public void AddProject(Project project)
        {
            _context.Projects.Add(project);
            _context.SaveChanges();
        }



        // Lấy phòng ban theo ID
        public Department GetDepartmentById(string deptId)
        {
            return _context.Departments.FirstOrDefault(d => d.DeptId == deptId);
        }

        // Lấy tất cả các phòng ban
        public List<Department> GetAllDepartments()
        {
            return _context.Departments.ToList();
        }
        public void AddEmployeeToProject(string accountId, int projectId, string role)
        {
            var employeeProject = new EmployeeProject
            {
                AccountId = accountId,
                ProjectId = projectId,
                Role = role
            };

            _context.EmployeeProjects.Add(employeeProject);
            _context.SaveChanges();
        }
        public void UpdateProject(Project project, List<string> departmentIds)
        {
            // Lấy dự án từ CSDL theo ID
            var existingProject = _context.Projects
                .Include(p => p.Departments) // Bao gồm các phòng ban liên kết với dự án
                .FirstOrDefault(p => p.ProjectId == project.ProjectId);

            if (existingProject == null)
            {
                throw new KeyNotFoundException("Project not found.");
            }

            // Cập nhật thông tin cơ bản của dự án
            existingProject.ProjectName = project.ProjectName;
            existingProject.Description = project.Description;
            existingProject.Deadline = project.Deadline;
            existingProject.Status = project.Status;

            // Cập nhật danh sách phòng ban liên kết với dự án
            if (departmentIds != null && departmentIds.Any())
            {
                // Xóa các liên kết phòng ban cũ
                existingProject.Departments.Clear();

                // Thêm các phòng ban mới vào dự án
                foreach (var deptId in departmentIds)
                {
                    var department = _context.Departments.FirstOrDefault(d => d.DeptId == deptId);
                    if (department != null)
                    {
                        existingProject.Departments.Add(department);
                    }
                }
            }

            // Lưu các thay đổi vào CSDL
            _context.Projects.Update(project);
            _context.SaveChanges();
        }


        public void DeleteProject(int projectId)
        {
            // Tìm dự án theo ID, bao gồm các liên kết với các thực thể khác
            var project = _context.Projects
                .Include(p => p.Tasks)                 // Bao gồm các Tasks liên quan
                    .ThenInclude(t => t.Subtasks)       // Bao gồm các Subtasks liên quan
                .Include(p => p.Tasks)                 // Bao gồm Tasks để xóa liên kết với TaskEvaluation
                    .ThenInclude(t => t.TaskEvaluations)
                .Include(p => p.Departments)           // Bao gồm Departments liên kết với Project
                .Include(p => p.EmployeeProjects)      // Bao gồm các liên kết với nhân viên
                .FirstOrDefault(p => p.ProjectId == projectId);

            if (project != null)
            {
                // 1. Xóa tất cả các Subtasks và liên kết của chúng
                foreach (var task in project.Tasks)
                {
                    // Xóa các Subtask và các liên kết của chúng
                    if (task.Subtasks.Any())
                    {
                        _context.Subtasks.RemoveRange(task.Subtasks);

                        var subtaskAssignments = _context.SubtaskAssignments
                            .Where(sa => task.Subtasks.Select(s => s.SubtaskId).Contains(sa.SubtaskId))
                            .ToList();
                        _context.SubtaskAssignments.RemoveRange(subtaskAssignments);

                        var subtaskEvaluations = _context.SubtaskEvaluations
                            .Where(se => task.Subtasks.Select(s => s.SubtaskId).Contains(se.SubtaskId))
                            .ToList();
                        _context.SubtaskEvaluations.RemoveRange(subtaskEvaluations);
                    }

                    // Xóa các TaskEvaluations
                    var taskEvaluations = _context.TaskEvaluations
                        .Where(te => te.TaskId == task.TaskId)
                        .ToList();
                    _context.TaskEvaluations.RemoveRange(taskEvaluations);

     
                    // Xóa các liên kết giữa Task và Department trong bảng DepartmentTask
                    var departmentTasks = _context.DepartmentTasks
                        .Where(dt => dt.TaskId == task.TaskId)
                        .ToList();
                    _context.DepartmentTasks.RemoveRange(departmentTasks);
                }

                // 2. Xóa tất cả các Tasks liên quan
                _context.Tasks.RemoveRange(project.Tasks);

                // 3. Xóa các liên kết với phòng ban trong bảng DepartmentProject (nếu cần thiết)
                project.Departments.Clear();

                // 4. Xóa tất cả các EmployeeProjects liên quan
                if (project.EmployeeProjects.Any())
                {
                    _context.EmployeeProjects.RemoveRange(project.EmployeeProjects);
                }

                // 5. Xóa dự án
                _context.Projects.Remove(project);

                // 6. Lưu các thay đổi vào cơ sở dữ liệu
                _context.SaveChanges();
            }
        }

    }
}