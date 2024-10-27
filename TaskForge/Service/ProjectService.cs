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
        public List<Project> GetAllProjectsByManagerAccountId(string accountId)
        {
            return _projectRepository.GetAllProjectsByAccount(accountId);
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

        // Lấy phòng ban theo ID
        public Department GetDepartmentById(string deptId)
        {
            return _projectRepository.GetDepartmentById(deptId);
        }

        // Thêm nhân viên vào dự án
        public void AddEmployeeToProject(string accountId, int projectId, string role)
        {
            _projectRepository.AddEmployeeToProject(accountId, projectId, role);
        }

        // Cập nhật dự án và danh sách phòng ban liên quan
        public void UpdateProject(Project project, List<string> departmentIds)
        {
            _projectRepository.UpdateProject(project, departmentIds);
        }

        // Xóa dự án
        public void DeleteProject(int projectId)
        {
            _projectRepository.DeleteProject(projectId);  // Gọi phương thức repository để xóa dự án
        }
        public void UpdateProjectDepartments(Project project, List<string> selectedDepartments)
        {
            // Xóa tất cả các phòng ban hiện đang liên kết với dự án
            project.Departments.Clear();

            // Thêm lại các phòng ban được chọn vào dự án
            foreach (var deptId in selectedDepartments)
            {
                var department = _projectRepository.GetDepartmentById(deptId);
                if (department != null)
                {
                    project.Departments.Add(department);
                }
            }

            // Lưu thay đổi vào cơ sở dữ liệu, truyền danh sách departmentIds
            _projectRepository.UpdateProject(project, selectedDepartments);
        }

        public List<string> GetDepartmentsWithAssignedTasks(int projectId)
        {
            return _projectRepository.GetDepartmentsWithAssignedTasks(projectId);
        }

    }
}
