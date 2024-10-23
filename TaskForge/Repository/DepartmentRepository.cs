using TaskForge.Models;
using TaskForge.DBContext;
using System.Collections.Generic;
using System.Linq;

namespace TaskForge.Repository
{
    public class DepartmentRepository
    {
        private readonly TaskForgeContext _context;

        public DepartmentRepository(TaskForgeContext context)
        {
            _context = context;
        }

        // Lấy tất cả phòng ban
        public List<Department> GetAllDepartments()
        {
            return _context.Departments.ToList();
        }

        // Lấy tất cả DepartmentId từ bảng Departments
        public IEnumerable<string> GetAllDepartmentIds()
        {
            return _context.Departments.Select(d => d.DeptId).ToList();
        }
        // Lấy phòng ban theo ID
        public Department GetDepartmentById(string deptId)
        {
            return _context.Departments.FirstOrDefault(d => d.DeptId == deptId);
        }

        // Tạo phòng ban mới
        public void CreateDepartment(Department department)
        {
            _context.Departments.Add(department);
            _context.SaveChanges();
        }

        // Cập nhật phòng ban
        public void UpdateDepartment(Department department)
        {
            _context.Departments.Update(department);
            _context.SaveChanges();
        }

        // Xóa phòng ban
        public void DeleteDepartment(string deptId)
        {
            var department = GetDepartmentById(deptId);
            if (department != null)
            {
                _context.Departments.Remove(department);
                _context.SaveChanges();
            }
        }
    }
}
