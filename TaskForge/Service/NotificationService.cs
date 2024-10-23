using TaskForge.Models;

namespace TaskForge.Service
{
    public class NotificationService
    {
        public void SendNotification(string email, string message)
        {
            // Logic gửi thông báo đến người dùng (ví dụ qua email hoặc hệ thống thông báo)
            // Giả định sử dụng hệ thống gửi email (thay bằng logic thực tế)
            Console.WriteLine($"Gửi thông báo đến {email}: {message}");
        }
    }
}
