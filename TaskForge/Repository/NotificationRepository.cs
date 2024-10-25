using TaskForge.DBContext;
using TaskForge.Models;

namespace TaskForge.Repository
{
    public class NotificationRepository
    {
        private readonly TaskForgeContext _context;

        public NotificationRepository(TaskForgeContext context)
        {
            _context = context;
        }

        // Thêm một thông báo mới vào cơ sở dữ liệu
        public void AddNotification(Notification notification)
        {
            _context.Notifications.Add(notification);
            _context.SaveChanges();
        }

        // Lấy danh sách thông báo chưa đọc của một tài khoản cụ thể
        public List<Notification> GetUnreadNotifications(string accountId)
        {
            return _context.Notifications
                           .Where(n => n.AccountId == accountId && n.IsRead == false)
                           .OrderByDescending(n => n.CreatedDate)
                           .ToList();
        }

        // Đánh dấu một thông báo là đã đọc
        public void MarkAsRead(int notificationId)
        {
            var notification = _context.Notifications.Find(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                _context.SaveChanges();
            }
        }

        public List<Notification> GetRecentNotifications(string accountId, int count)
        {
            return _context.Notifications
                           .Where(n => n.AccountId == accountId)
                           .OrderByDescending(n => n.CreatedDate)
                           .Take(count)
                           .ToList();
        }
        // Lấy tất cả các thông báo của một tài khoản, sắp xếp theo ngày mới nhất
        public List<Notification> GetAllNotificationsByAccountId(string accountId)
        {
            return _context.Notifications
                           .Where(n => n.AccountId == accountId)
                           .OrderByDescending(n => n.CreatedDate)
                           .ToList();
        }
    }
}