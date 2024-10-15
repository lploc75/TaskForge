using TaskForge.Models;
using TaskForge.DBContext;
using Microsoft.EntityFrameworkCore;
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
            // Use Include to load related Account data
            return _context.Employees
                .Include(e => e.Account)
                .FirstOrDefault(e => e.AccountId == accountId);
        }
    }
}
