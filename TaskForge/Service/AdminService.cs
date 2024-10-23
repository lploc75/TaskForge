using TaskForge.Models;
using TaskForge.Repository;

namespace TaskForge.Service
{
    public class AdminService
    {
        private readonly AccountRepository _accountRepository;
        private readonly TeamRepository _teamRepository;
        public AdminService(AccountRepository accountRepository, TeamRepository teamRepository)
        {
            _accountRepository = accountRepository;
            _teamRepository = teamRepository;
        }

        // Phương thức để lấy thông tin tài khoản Admin dựa vào AccountId
        public Account GetAdminInfo(string accountId)
        {
            return _accountRepository.GetAccountById(accountId);
        }
        public List<Account> GetAllAccounts()
        {
            return _accountRepository.GetAllAccounts();
        }
        public List<Team> GetAllTeams()
        {
            return _teamRepository.GetAllTeams();
        }

        // Lấy tài khoản theo ID
        public Account GetAccountById(string accountId)
        {
            return _accountRepository.GetAccountById(accountId);
        }

        public void CreateAccount(string Username, string Password, string Email, string PhoneNumber)
        {
            // Tạo AccountId mới
            var accountId = _accountRepository.GenerateAccountId();

            // Tạo đối tượng Account mới
            var account = new Account
            {
                AccountId = accountId,
                Username = Username,
                Password = Password,
                Email = Email,
                PhoneNumber = PhoneNumber
            };

            // Gọi repository để lưu tài khoản mới
            _accountRepository.AddAccount(account);
        }


        // Chỉnh sửa tài khoản
        public void EditAccount(string accountId, string Username, string Password, string Email, string PhoneNumber)
        {
            // Lấy account từ repository
            var account = _accountRepository.GetAccountById(accountId);

            if (account != null)
            {
                // Cập nhật thông tin tài khoản
                account.Username = Username;
                account.Password = Password;
                account.Email = Email;
                account.PhoneNumber = PhoneNumber;

                // Gọi repository để lưu thay đổi
                _accountRepository.UpdateAccount(account);
            }
        }

        // Xóa tài khoản
        public void DeleteAccount(string accountId)
        {
            // Kiểm tra tài khoản có tồn tại không
            var account = _accountRepository.GetAccountById(accountId);

            if (account != null)
            {
                // Gọi repository để xóa tài khoản
                _accountRepository.DeleteAccount(account);
            }
        }
    }
}
