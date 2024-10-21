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
        public StaffAndLeader GetStaffByAccountId(string accountId)
        {
            return _context.StaffAndLeaders.FirstOrDefault(s => s.AccountId == accountId);
        }
        public void RecordCreditExchange(CreditExchange creditExchange)
        {
            _context.CreditExchanges.Add(creditExchange);
            _context.SaveChanges();
        }
        public void UpdateStaff(StaffAndLeader staff)
        {
            _context.StaffAndLeaders.Update(staff);
            _context.SaveChanges();
        }
        public CreditExchange GetCreditExchangeById(int exchangeId)
        {
            return _context.CreditExchanges
                .Include(e => e.Account) // Ensure the Account entity is included
                .FirstOrDefault(e => e.ExchangeId == exchangeId);
        }
    }
}
