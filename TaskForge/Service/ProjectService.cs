using TaskForge.Models;
using TaskForge.Repository;
using System.Collections.Generic;

namespace TaskForge.Service
{
    public class ProjectService
    {
        private readonly ProjectRepository _projectRepository;

        public ProjectService(ProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        //// Lấy dự án theo trạng thái và AccountId của Manager
        //public List<Project> GetProjectsByStatusAndManager(string status, string accountId) // Dùng AccountId thay cho EmployeeId
        //{
        //    return _projectRepository.GetProjectsByStatusAndManager(status, accountId);
        //}

        // Thêm dự án mới vào cơ sở dữ liệu
        public void AddNewProject(Project newProject)
        {
            _projectRepository.AddProject(newProject);
        }
    }
}
