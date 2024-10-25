using System.Collections.Generic;
using TaskForge.Models;
using TaskForge.Repository;

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
    }
}
