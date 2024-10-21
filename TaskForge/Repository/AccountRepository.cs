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

        // Method to get role by AccountId from the employee table
        public async Task<string> GetRoleByAccountIdAsync(string accountId)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.AccountId == accountId);

            return employee?.Role;
        }

        public Account GetAccountByEmail(string email)
        {
            return _context.Accounts.FirstOrDefault(a => a.Email == email);
        }

        public Account GetAccountById(string accountId)
        {
            return _context.Accounts.FirstOrDefault(a => a.AccountId == accountId);
        }
        public void UpdateAccount(Account account)
        {
            _context.Accounts.Update(account);
            _context.SaveChanges();
        }
    }
}
