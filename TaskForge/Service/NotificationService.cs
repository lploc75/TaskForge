using TaskForge.Models;
using TaskForge.Repository;

namespace TaskForge.Service
{
    public class NotificationService
    {
        private readonly NotificationRepository _notificationRepository;
        public NotificationService(NotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }
        // Tạo thông báo mới với logic nghiệp vụ
        public void CreateNotification(string accountId, string message)
        {
            var notification = new Notification
            {
                AccountId = accountId,
                Message = message,
                CreatedDate = DateTime.Now,
                IsRead = false
            };
            _notificationRepository.AddNotification(notification);
        }

        // Lấy danh sách thông báo chưa đọc cho một tài khoản cụ thể
        public List<Notification> GetUnreadNotifications(string accountId)
        {
            return _notificationRepository.GetUnreadNotifications(accountId);
        }

        // Đánh dấu một thông báo là đã đọc
        public void MarkAsRead(int notificationId)
        {
            _notificationRepository.MarkAsRead(notificationId);
        }

        public List<Notification> GetRecentNotifications(string accountId, int count)
        {
            return _notificationRepository.GetRecentNotifications(accountId, count);
        }
        // Lấy tất cả các thông báo cho một tài khoản, sắp xếp theo ngày mới nhất
        public List<Notification> GetAllNotificationsByAccountId(string accountId)
        {
            return _notificationRepository.GetAllNotificationsByAccountId(accountId);
        }
    }
}
