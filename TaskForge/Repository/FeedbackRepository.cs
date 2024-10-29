using TaskForge.Models;
using TaskForge.DBContext;
using System.Collections.Generic;
using System.Linq;

namespace TaskForge.Repository
{
    public class FeedbackRepository
    {
        private readonly TaskForgeContext _context;

        public FeedbackRepository(TaskForgeContext context)
        {
            _context = context;
        }

        // Lấy tất cả phản hồi
        public List<Feedback> GetAllFeedbacks()
        {
            return _context.Feedbacks.ToList();
        }

        // Tạo phản hồi mới
        public void CreateFeedback(Feedback feedback)
        {
            _context.Feedbacks.Add(feedback);
            _context.SaveChanges();
        }

        // Xóa phản hồi
        public void DeleteFeedback(int feedbackId)
        {
            var feedback = _context.Feedbacks.FirstOrDefault(f => f.FeedbackId == feedbackId);
            if (feedback != null)
            {
                _context.Feedbacks.Remove(feedback);
                _context.SaveChanges();
            }
        }
        public int GetMaxFeedbackId()
        {
            // Trả về feedback_id cao nhất hoặc 0 nếu bảng trống
            return _context.Feedbacks.Max(f => (int?)f.FeedbackId) ?? 0;
        }
    }
}
