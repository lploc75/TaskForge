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
            return _context.Projects.FirstOrDefault(p => p.ProjectId == projectId);
        }

        // Thêm dự án mới vào database
        public void AddProject(Project project)
        {
            _context.Projects.Add(project);
            _context.SaveChanges();
        }

        // Cập nhật dự án với danh sách phòng ban
        public void UpdateProjectDepartments(Project project)
        {
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
        // Cập nhật dự án trong cơ sở dữ liệu
        public void UpdateProject(Project project)
        {
            _context.Projects.Update(project);
            _context.SaveChanges();
        }

        public void DeleteProject(int projectId)
        {
            // Tìm dự án theo ID, bao gồm các liên kết với các thực thể khác
            var project = _context.Projects
                .Include(p => p.Departments)   // Bao gồm các phòng ban liên quan
                .Include(p => p.Tasks)         // Bao gồm các task liên quan
                .Include(p => p.EmployeeProjects)  // Bao gồm các liên kết với nhân viên
                .FirstOrDefault(p => p.ProjectId == projectId);

            if (project != null)
            {
                // 1. Xóa các liên kết với phòng ban
                if (project.Departments != null && project.Departments.Any())
                {
                    project.Departments.Clear(); // Xóa tất cả liên kết với các phòng ban
                }

                // 2. Xóa các tasks liên quan đến dự án
                if (project.Tasks != null && project.Tasks.Any())
                {
                    _context.Tasks.RemoveRange(project.Tasks);  // Xóa tất cả tasks liên quan
                }

                // 3. Xóa các liên kết với nhân viên
                if (project.EmployeeProjects != null && project.EmployeeProjects.Any())
                {
                    _context.EmployeeProjects.RemoveRange(project.EmployeeProjects);  // Xóa tất cả các EmployeeProjects liên quan
                }
                // 4. Xóa dự án
                _context.Projects.Remove(project);

                // 5. Lưu các thay đổi vào cơ sở dữ liệu
                _context.SaveChanges();
            }
        }
    }
}