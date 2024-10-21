using System.Collections.Generic;
using TaskForge.Models;
using TaskForge.Repository;

namespace TaskForge.Service
{
    public class ProjectService
    {
        private readonly ProjectRepository _projectRepository;

        public ProjectService(ProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        // Lấy danh sách dự án mà nhân viên có vai trò là "Manager"
        public List<Project> GetProjectsByStatusAndAccount(string status, string accountId)
        {
            return _projectRepository.GetProjectsByStatusAndAccount(status, accountId);
        }

        // Lấy chi tiết dự án theo ID
        public Project GetProjectById(int projectId)
        {
            return _projectRepository.GetProjectById(projectId);
        }

        // Lấy tất cả các phòng ban
        public List<Department> GetAllDepartments()
        {
            return _projectRepository.GetAllDepartments();
        }

        // Thêm dự án mới
        public void AddProject(Project project)
        {
            _projectRepository.AddProject(project);
        }

        // Cập nhật danh sách phòng ban cho dự án
        public void UpdateProjectDepartments(Project project)
        {
            _projectRepository.UpdateProjectDepartments(project);
        }

        // Lấy phòng ban theo ID
        public Department GetDepartmentById(string deptId)
        {
            return _projectRepository.GetDepartmentById(deptId);
        }

        public void AddEmployeeToProject(string accountId, int projectId, string role)
        {
            _projectRepository.AddEmployeeToProject(accountId, projectId, role);
        }
        // Cập nhật dự án
        public void UpdateProject(Project project)
        {
            _projectRepository.UpdateProject(project);
        }

        public void DeleteProject(int projectId)
        {
            _projectRepository.DeleteProject(projectId);  // Gọi phương thức repository để xóa dự án
        }

    }
}
