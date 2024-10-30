using TaskForge.Models;
using TaskForge.Repository;

namespace TaskForge.Service
{
    public class FeedbackService
    {
        private readonly FeedbackRepository _feedbackRepository;

        public FeedbackService(FeedbackRepository feedbackRepository)
        {
            _feedbackRepository = feedbackRepository;
        }

        // Lấy tất cả phản hồi
        public List<Feedback> GetAllFeedbacks()
        {
            return _feedbackRepository.GetAllFeedbacks();
        }

        // Tạo phản hồi mới
        public void CreateFeedback(Feedback feedback)
        {
            _feedbackRepository.CreateFeedback(feedback);
        }

        // Xóa phản hồi
        public void DeleteFeedback(int feedbackId)
        {
            _feedbackRepository.DeleteFeedback(feedbackId);
        }
        public int GetNextFeedbackId()
        {
            // Lấy feedback_id lớn nhất trong bảng Feedback
            var maxId = _feedbackRepository.GetMaxFeedbackId();
            return maxId + 1;
        }
    }
}