    using System.Collections.Generic;
    using TaskForge.Models;
    using TaskForge.Repository;

    namespace TaskForge.Service
    {
        public class TaskService
        {
            private readonly TaskRepository _taskRepository;

            public TaskService(TaskRepository taskRepository)
            {
                _taskRepository = taskRepository;
            }

            // Lấy tất cả các task theo ProjectId
            public List<TaskForge.Models.Task> GetTasksByProjectId(int projectId)
            {
                return _taskRepository.GetTasksByProjectId(projectId);
            }

            // Tạo một task mới cho dự án
            public void CreateTask(TaskForge.Models.Task task, List<string> departmentIds)
            {
                _taskRepository.CreateTask(task, departmentIds);
            }
        }
    }
