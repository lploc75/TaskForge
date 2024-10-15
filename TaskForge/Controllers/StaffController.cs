using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskForge.Service;
using TaskForge.Models;

namespace TaskForge.Controllers
{
    [Authorize]
    public class StaffController : Controller
    {
        private readonly EmployeeService _employeeService;

        public StaffController(EmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public IActionResult Index()
        {
            string accountId = User.FindFirst("AccountId")?.Value;

            if (string.IsNullOrEmpty(accountId))
            {
                // Xử lý khi không tìm thấy AccountID
                return RedirectToAction("Error", "Home");
            }

            Employee employee = _employeeService.GetEmployeeByAccountId(accountId);

            if (employee == null)
            {
                // Xử lý khi không tìm thấy dữ liệu Employee
                return RedirectToAction("Error", "Home");
            }

            return View(employee);
        }

    }
}
