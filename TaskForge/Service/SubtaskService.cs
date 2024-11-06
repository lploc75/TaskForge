using System.Collections.Generic;
using TaskForge.Models;
using TaskForge.Repository;
using X.PagedList;
using X.PagedList.Extensions;

namespace TaskForge.Service
{
    public class SubtaskService
    {
        private readonly SubtaskRepository _subtaskRepository;

        public SubtaskService(SubtaskRepository subtaskRepository)
        {
            _subtaskRepository = subtaskRepository;
        }

        public List<Subtask> GetSubtasksByTaskId(string taskId)
        {
            return _subtaskRepository.GetSubtasksByTaskId(taskId);
        }

        public Subtask GetSubtaskById(string subtaskId)
        {
            return _subtaskRepository.GetSubtaskById(subtaskId);
        }

        public void CreateSubtask(Subtask subtask)
        {
            _subtaskRepository.CreateSubtask(subtask);
        }

        public void UpdateSubtask(Subtask subtask)
        {
            _subtaskRepository.UpdateSubtask(subtask);
        }

        public void DeleteSubtask(string subtaskId)
        {
            _subtaskRepository.DeleteSubtask(subtaskId);
        }
        public List<Employee> GetEmployeesBySubtaskId(string subtaskId)
        {
            return _subtaskRepository.GetEmployeesBySubtaskId(subtaskId);
        }
        public void SaveEvaluation(SubtaskEvaluation evaluation)
        {
            _subtaskRepository.SaveEvaluation(evaluation);
        }
        public PagedList<Subtask> GetFilteredSubtasks(string status, int? priority, int? difficulty, DateTime? assignmentDateMin, DateTime? assignmentDateMax, DateTime? deadlineMin, DateTime? deadlineMax, string submission, string taskId, string teamId, int page, int pageSize)
        {
            var subtasks = _subtaskRepository.FilterSubtasks(status, priority, difficulty, assignmentDateMin, assignmentDateMax, deadlineMin, deadlineMax, submission, taskId, teamId);

            // Phân trang logic
            var pagedSubtasks = subtasks.ToPagedList(page, pageSize);
            return (PagedList<Subtask>)pagedSubtasks;
        }
        public bool RejectSubtaskAssignment(string subtaskId)
        {
            try
            {
                _subtaskRepository.RemoveSubtaskAssignment(subtaskId);
                return true; // Successful
            }
            catch
            {
                return false; // Failed
            }
        }
        public bool UpdateSubtaskStatus(string subtaskId, string status)
        {
            // Gọi repository để lấy subtask
            var subtask = _subtaskRepository.GetSubtaskById(subtaskId);
            if (subtask != null)
            {
                subtask.Status = status;
                if (status == "Pending")
                {
                    subtask.SubmissionDate = DateTime.Now; // Cập nhật SubmissionDate
                }
                else
                {
                    subtask.SubmissionDate = null;
                }
                _subtaskRepository.UpdateSubtask(subtask);
                return true;
            }
            return false;
        }
    }
}