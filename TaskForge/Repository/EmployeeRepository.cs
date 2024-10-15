using TaskForge.Models;
using TaskForge.DBContext;
using System.Linq;

namespace TaskForge.Repository
{
    public class EmployeeRepository
    {
        private readonly TaskForgeContext _context;

        public EmployeeRepository(TaskForgeContext context)
        {
            _context = context;
        }

        public Employee GetEmployeeByAccountId(string accountId)
        {
            return _context.Employees.FirstOrDefault(e => e.AccountId == accountId);
        }
    }
}
