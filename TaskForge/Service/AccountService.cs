using Microsoft.Extensions.Caching.Memory;
using TaskForge.Models;
using TaskForge.Repository;

namespace TaskForge.Service
{
    public class AccountService
    {
        private readonly AccountRepository _accountRepository;
        private readonly EmailService _emailService;
        private readonly IMemoryCache _cache;
        public AccountService(AccountRepository accountRepository, EmailService emailService, IMemoryCache cache)
        {
            _accountRepository = accountRepository;
            _emailService = emailService;
            _cache = cache;
        }
        // Phương thức xử lý logic đăng nhập
        public async Task<Account> LoginAsync(string username, string password)
        {
            return await _accountRepository.ValidateAsync(username, password);
        }
        // Phương thức lấy role từ bảng employee
        public async Task<string> GetRoleByAccountIdAsync(string accountId)
        {
            return await _accountRepository.GetRoleByAccountIdAsync(accountId);
        }
        // Phương thức quên mật khẩu
        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var account = _accountRepository.GetAccountByEmail(email);
            if (account == null)
            {
                return false; // Email không tồn tại
            }

            // Tạo mã reset và lưu vào MemoryCache
            var resetToken = Guid.NewGuid().ToString();
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(1)); // Thời gian hết hạn của token là 1 giờ

            _cache.Set(resetToken, account.AccountId, cacheEntryOptions);

            // Tạo liên kết reset mật khẩu
            var resetLink = $"https://localhost:7286/Account/ResetPassword?token={resetToken}";
            var subject = "Password Reset";
            var body = $"<p>Nhấn vào liên kết sau để đặt lại mật khẩu của bạn: <a href='{resetLink}'>Reset Password</a></p>";

            // Gửi email reset
            await _emailService.SendEmailAsync(email, subject, body);

            return true;
        }

        // Phương thức xác thực token reset mật khẩu
        public bool ValidatePasswordResetToken(string token)
        {
            return _cache.TryGetValue(token, out _);
        }

        // Phương thức đặt lại mật khẩu
        public void UpdatePassword(string token, string newPassword)
        {
            // Kiểm tra xem token có trong MemoryCache không
            if (_cache.TryGetValue(token, out string accountId))
            {
                // Lấy tài khoản từ database bằng accountId
                var account = _accountRepository.GetAccountById(accountId);
                if (account != null)
                {
                    // Cập nhật mật khẩu (có thể thêm mã hóa nếu cần)
                    account.Password = newPassword;
                    _accountRepository.UpdateAccount(account);

                    // Xóa token khỏi cache sau khi dùng
                    _cache.Remove(token);
                }
            }
        }
        public async Task<bool> ChangePasswordAsync(string accountId, string currentPassword, string newPassword)
        {
            var account = _accountRepository.GetAccountById(accountId);
            if (account == null || account.Password != currentPassword)
            {
                return false; // Current password doesn't match
            }

            // Update password
            account.Password = newPassword;
            _accountRepository.UpdateAccount(account);

            return true;
        }
    }
}
