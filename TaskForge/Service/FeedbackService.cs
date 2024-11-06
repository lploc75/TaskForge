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
        public string SubmitFeedback(string accountId, string context)
        {
            if (string.IsNullOrWhiteSpace(context))
            {
                return "Please enter your feedback before submitting.";
            }

            int newFeedbackId = GetNextFeedbackId();

            var feedback = new Feedback
            {
                FeedbackId = newFeedbackId,
                Context = context,
                DateSubmitted = DateTime.Now,
                AccountId = accountId
            };

            CreateFeedback(feedback);

            return "Thank you for your feedback!";
        }

    }
}
