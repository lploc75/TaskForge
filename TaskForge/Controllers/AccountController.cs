using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskForge.Models;
using TaskForge.Service;

namespace TaskForge.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountService _accountService;

        // Constructor for dependency injection
        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View("ForgotPassword");  // Hiển thị ForgotPassword.cshtml
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var result = await _accountService.ForgotPasswordAsync(email);
            if (!result)
            {
                ModelState.AddModelError("", "Email không tồn tại trong hệ thống.");
                return View("ForgotPassword");
            }

            ViewBag.Message = "Email khôi phục mật khẩu đã được gửi.";
            return View("ForgotPasswordConfirmation");  // Hiển thị ForgotPasswordConfirmation.cshtml
        }

        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            if (!_accountService.ValidatePasswordResetToken(token))
            {
                return RedirectToAction("ForgotPassword");
            }

            ViewBag.Token = token;
            return View("ResetPassword");  // Hiển thị ResetPassword.cshtml
        }

        [HttpPost]
        public IActionResult ResetPassword(string token, string newPassword, string confirmPassword)
        {
            if (!_accountService.ValidatePasswordResetToken(token))
            {
                return RedirectToAction("ForgotPassword");
            }

            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("", "Mật khẩu mới và xác nhận mật khẩu không khớp.");
                ViewBag.Token = token;
                return View("ResetPassword");
            }

            _accountService.UpdatePassword(token, newPassword);

            ViewBag.Message = "Mật khẩu của bạn đã được cập nhật thành công.";
            return RedirectToAction("Login");
        }


        [HttpPost]
        public async Task<IActionResult> Login(Account account)
        {
            try
            {
                var validatedAccount = await _accountService.LoginAsync(account.Username, account.Password);
                if (validatedAccount == null)
                {
                    TempData["Message"] = "Sai tên đăng nhập hoặc mật khẩu.";
                    // Chuyển hướng về trang danh sách tài khoản sau khi tạo xong
                    return View(account);
                }

                // Lấy role từ bảng employee
                var role = await _accountService.GetRoleByAccountIdAsync(validatedAccount.AccountId);
                if (role == null)
                {
                    ModelState.AddModelError(string.Empty, "Không tìm thấy vai trò cho tài khoản này.");
                    return View(account);
                }

                // Xử lý đăng nhập thành công
                var claims = new List<Claim>
                {
                    new Claim("AccountId", validatedAccount.AccountId.ToString()),
                    new Claim(ClaimTypes.Role, role)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                
                // Lưu layout vào session dựa trên role
                if (role == "Leader")
                {
                    HttpContext.Session.SetString("Layout", "_LeaderLayout");
                }
                else if (role == "Staff")
                {
                    HttpContext.Session.SetString("Layout", "_StaffLayout");
                }

                // Chuyển hướng dựa trên Role
                return role switch
                {
                    "Staff" => RedirectToAction("Index", "StaffAndLeader"),
                    "Admin" => RedirectToAction("Index", "Admin"),
                    "Manager" => RedirectToAction("Index", "Manager"),
                    "Leader" => RedirectToAction("Index", "StaffAndLeader"),
                    "Department Head" => RedirectToAction("Index", "DepartmentHead"),
                    _ => RedirectToAction("Index", "Home") // Mặc định về trang chủ nếu không khớp role
                };
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(account);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("Layout"); // Xóa layout trong session
            return RedirectToAction("Login", "Account");
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                return Json(new { success = false, message = "Mật khẩu mới và xác nhận mật khẩu không khớp." });
            }

            var accountId = User.FindFirstValue("AccountId");
            var result = await _accountService.ChangePasswordAsync(accountId, currentPassword, newPassword);

            if (!result)
            {
                return Json(new { success = false, message = "Mật khẩu hiện tại không chính xác." });
            }

            return Json(new { success = true, message = "Mật khẩu của bạn đã được cập nhật thành công." });
        }
    }
}
