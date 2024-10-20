using TaskForge.Models;
using TaskForge.DBContext;
using System.Collections.Generic;
using System.Linq;

namespace TaskForge.Repository
{
    public class ProjectRepository
    {
        private readonly TaskForgeContext _context;

        public ProjectRepository(TaskForgeContext context)
        {
            _context = context;
        }

        // Lấy danh sách dự án theo trạng thái và AccountId của Manager
        //public List<Project> GetProjectsByStatusAndManager(string status, string accountId) // Dùng AccountId thay cho EmployeeId
        //{
        //    return _context.Projects
        //        .Where(p => p.Status == status && p.Account.Any(e => e.AccountId == accountId)) // Kiểm tra trạng thái và AccountId
        //        .ToList();
        //}

        // Thêm dự án mới vào cơ sở dữ liệu
        public void AddProject(Project project)
        {
            _context.Projects.Add(project);
            _context.SaveChanges();
        }

        // Các phương thức khác liên quan đến dự án
    }
}
