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

        // Lấy tài khoản theo ID
        public Account GetAccountById(string accountId)
        {
            return _context.Accounts.FirstOrDefault(a => a.AccountId == accountId);
        }

        // Cập nhật tài khoản trong DB
        public void UpdateAccount(Account account)
        {
            _context.Accounts.Update(account);
            _context.SaveChanges();
        }

        // Lấy tất cả tài khoản
        public List<Account> GetAllAccounts()
        {
            return _context.Accounts.ToList();
        }
        // Phương thức sinh account_id mới
        public string GenerateAccountId()
        {
            // Lấy AccountId lớn nhất hiện tại từ cơ sở dữ liệu
            var lastAccountId = _context.Accounts
                                        .OrderByDescending(a => a.AccountId)
                                        .Select(a => a.AccountId)
                                        .FirstOrDefault();

            if (lastAccountId != null)
            {
                // Tách phần số từ AccountId hiện tại, ví dụ ACC001 -> 1
                var lastNumericPart = int.Parse(lastAccountId.Substring(3));

                // Tăng số lên 1, ví dụ 1 -> 2
                var newNumericPart = lastNumericPart + 1;

                // Tạo AccountId mới với định dạng ACCxxx (với phần số có độ dài 3)
                var newAccountId = "ACC" + newNumericPart.ToString("D3"); // D3 nghĩa là định dạng số có 3 chữ số, ví dụ 001, 002, ...

                return newAccountId;
            }
            else
            {
                // Nếu không có AccountId nào, bắt đầu từ ACC001
                return "ACC001";
            }
        }
        // Thêm tài khoản mới vào DB
        public void AddAccount(Account account)
        {
            _context.Accounts.Add(account);
            _context.SaveChanges();
        }

        // Xóa tài khoản trong DB
        public void DeleteAccount(Account account)
        {
            _context.Accounts.Remove(account);
            _context.SaveChanges();
        }
    }
}
