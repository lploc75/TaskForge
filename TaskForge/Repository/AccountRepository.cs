using Microsoft.EntityFrameworkCore;
using TaskForge.DBContext;
using TaskForge.Models;

namespace TaskForge.Repository
{
    public class AccountRepository
    {
        private readonly TaskForgeContext _context;
        public AccountRepository(TaskForgeContext context)
        {
            _context = context;
        }
        // Method to validate the account
        public async Task<Account> ValidateAsync(string username, string password)
        {
            return await _context.Accounts
                .FirstOrDefaultAsync(a => a.Username == username && a.Password == password);
        }
    }
}
