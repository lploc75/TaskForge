using System.ComponentModel.DataAnnotations;

namespace TaskForge.Models
{
    public class Account
    {
        [Key]  // Đặt thuộc tính này làm khóa chính

        public string account_id { get; set; }  // Thuộc tính này sẽ là khóa chính (primary key)

        public string username { get; set; }

        public string password { get; set; }
        public string email { get; set; }
        public string role { get; set; }
        public string phone_number { get; set; }
    }
}
