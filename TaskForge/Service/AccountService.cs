using TaskForge.Models;
using TaskForge.Repository;

namespace TaskForge.Service
{
    public class AccountService
    {
        private readonly AccountRepository _accountRepository;
        public AccountService(AccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        // Phương thức xử lý logic đăng nhập
        public async Task<Account> LoginAsync(string username, string password)
        {
            
            return await _accountRepository.ValidateAsync(username, password);
        }
    }
}
