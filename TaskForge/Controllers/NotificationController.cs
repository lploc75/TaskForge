using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskForge.Service;
using X.PagedList.Extensions;

namespace TaskForge.Controllers
{
    public class NotificationController : Controller
    {
        private readonly NotificationService _notificationService;

        public NotificationController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        // Hiển thị tất cả thông báo của người dùng
        public IActionResult Index(DateTime? startDate, DateTime? endDate, bool? isRead, int? page)
        {
            string accountId = User.FindFirst("AccountId")?.Value;

            // Lấy tất cả thông báo từ Service, giới hạn 150 thông báo gần nhất
            var notifications = _notificationService.GetAllNotificationsByAccountId(accountId)
                .OrderByDescending(n => n.CreatedDate)
                .Take(150)
                .ToList();

            // Lọc theo khoảng ngày nếu có
            if (startDate.HasValue)
            {
                notifications = notifications.Where(n => n.CreatedDate >= startDate).ToList();
            }
            if (endDate.HasValue)
            {
                notifications = notifications.Where(n => n.CreatedDate <= endDate).ToList();
            }

            // Lọc theo trạng thái đã đọc
            if (isRead.HasValue)
            {
                notifications = notifications.Where(n => n.IsRead == isRead).ToList();
            }

            // Thiết lập phân trang, mỗi trang có 10 thông báo
            int pageSize = 10;
            int pageNumber = (page ?? 1); // Nếu không có số trang, mặc định là 1

            // Áp dụng phân trang sau khi đã lọc
            var pagedNotifications = notifications.ToPagedList(pageNumber, pageSize);

            return View(pagedNotifications);
        }

        // Đánh dấu tất cả thông báo là đã đọc
        [HttpPost]
        public IActionResult MarkAsRead(int notificationId)
        {
            _notificationService.MarkAsRead(notificationId); // Gọi service để đánh dấu là đã đọc
            return RedirectToAction("Index"); // Điều hướng về trang tất cả thông báo
        }

    }
}
