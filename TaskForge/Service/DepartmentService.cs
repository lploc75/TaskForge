using TaskForge.Models;
using System.Collections.Generic;
using TaskForge.Repository;

namespace TaskForge.Service
{
    public class DepartmentService
    {
        private readonly DepartmentRepository _departmentRepository;

        public DepartmentService(DepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public IEnumerable<Department> GetAllDepartments()
        {
            return _departmentRepository.GetAllDepartments();
        }

        public void CreateDepartment(Department department)
        {
            _departmentRepository.CreateDepartment(department);
        }

        public void UpdateDepartment(Department department)
        {
            _departmentRepository.UpdateDepartment(department);
        }

        public void DeleteDepartment(string departmentId)
        {
            _departmentRepository.DeleteDepartment(departmentId);
        }
    }
}
