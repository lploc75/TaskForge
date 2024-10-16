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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Account account)
        {
            try
            {
                var validatedAccount = await _accountService.LoginAsync(account.Username, account.Password);
                if (validatedAccount == null)
                {
                    ModelState.AddModelError(string.Empty, "Sai tên đăng nhập hoặc mật khẩu.");
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

                // Chuyển hướng dựa trên Role
                return role switch
                {
                    "Staff" => RedirectToAction("Index", "Staff"),
                    "Admin" => RedirectToAction("Index", "Admin"),
                    "Manager" => RedirectToAction("Index", "Manager"),
                    "Leader" => RedirectToAction("Index", "Leader"),
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
            return RedirectToAction("Login", "Account");
        }

    }
}
