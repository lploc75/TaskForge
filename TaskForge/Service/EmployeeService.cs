using TaskForge.Models;
using TaskForge.Repository;

namespace TaskForge.Service
{
    public class EmployeeService
    {
        private readonly EmployeeRepository _employeeRepository;

        public EmployeeService(EmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public Employee GetEmployeeByAccountId(string accountId)
        {
            return _employeeRepository.GetEmployeeByAccountId(accountId);
        }
    }
}
