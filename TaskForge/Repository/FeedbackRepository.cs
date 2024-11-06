using TaskForge.Models;
using TaskForge.DBContext;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace TaskForge.Repository
{
    public class FeedbackRepository
    {
        private readonly TaskForgeContext _context;

        public FeedbackRepository(TaskForgeContext context)
        {
            _context = context;
        }

        // Lấy tất cả phản hồi bằng SQL trực tiếp
        public List<Feedback> GetAllFeedbacks()
        {
            return _context.Feedbacks
                           .FromSqlRaw("SELECT * FROM Feedback")
                           .ToList();
        }

        // Tạo phản hồi mới bằng SQL trực tiếp
        public void CreateFeedback(Feedback feedback)
        {
            var maxFeedbackId = GetMaxFeedbackId() + 1;
            _context.Database.ExecuteSqlRaw(
                "INSERT INTO Feedback (feedback_id, context, date_submitted, account_id) VALUES ({0}, {1}, {2}, {3})",
                maxFeedbackId, feedback.Context, feedback.DateSubmitted, feedback.AccountId
            );
        }

        // Xóa phản hồi bằng SQL trực tiếp
        public void DeleteFeedback(int feedbackId)
        {
            _context.Database.ExecuteSqlRaw(
                "DELETE FROM Feedback WHERE feedback_id = {0}", feedbackId);
        }

        // Lấy FeedbackId cao nhất bằng SQL trực tiếp
        public int GetMaxFeedbackId()
        {
            var maxFeedbackId = _context.Feedbacks
                                        .FromSqlRaw("SELECT MAX(feedback_id) AS feedback_id FROM Feedback")
                                        .Select(f => f.FeedbackId)
                                        .FirstOrDefault();

            return maxFeedbackId;
        }
    }
}