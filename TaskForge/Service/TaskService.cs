    using System.Collections.Generic;
    using TaskForge.Models;
    using TaskForge.Repository;

    namespace TaskForge.Service
    {
        public class TaskService
        {
            private readonly TaskRepository _taskRepository;
            private readonly EmployeeRepository _employeeRepository;
            public TaskService(TaskRepository taskRepository, EmployeeRepository employeeRepository)
            {
                _taskRepository = taskRepository;
                _employeeRepository = employeeRepository;
            }

            // Lấy tất cả các task theo ProjectId
            public List<TaskForge.Models.Task> GetTasksByProjectId(int projectId)
            {
                return _taskRepository.GetTasksByProjectId(projectId);
            }
            public void RejectSubtaskAssignment(string subtaskId)
            {
            _taskRepository.RemoveSubtaskAssignment(subtaskId);
            }
            // Tạo một task mới cho dự án
            public void CreateTask(TaskForge.Models.Task task, List<string> departmentIds)
            {
                _taskRepository.CreateTask(task, departmentIds);
            }
            public TaskForge.Models.Task GetTaskById(string taskId)
            {
            return _taskRepository.GetTaskById(taskId);
            }
            public void UpdateTask(TaskForge.Models.Task task)
            {
                _taskRepository.UpdateTask(task);
            }
            public void DeleteTask(string taskId)
            {
                _taskRepository.DeleteTask(taskId);
            }
            public void AddPersonalTask(PersonalTask ptask)
            {
            _taskRepository.AddPersonalTask(ptask);
            }

            // Lấy PersonalTask theo Id
            public PersonalTask GetPersonalTaskById(string PtaskId)
            {
                return _taskRepository.GetPersonalTaskById(PtaskId);
            }
            
            // Cập nhật thông tin của Personal Task
            public void UpdatePersonalTask(PersonalTask personalTask)
            {
                _taskRepository.UpdatePersonalTask(personalTask);
            }
            public void DeletePersonalTask(string ptaskId)
            {
                var task = _taskRepository.GetPersonalTasksById(ptaskId);
                if (task != null)
                {
                _taskRepository.DeletePersonalTasks(task);
                }
            }

        public List<Subtask> GetAllSubtasks()
        {
            return _taskRepository.GetAllSubtasks();
        }
        public List<PersonalTask> GetAllPersonalTasks()
        {
            return _taskRepository.GetAllPersonalTasks();
        }

        // Lấy danh sách các PersonalTask đã lọc cho người dùng
        public async Task<List<PersonalTask>> GetFilteredPersonalTasksForUserAsync(string accountId, string status, string priority, DateTime? deadline)
        {
            return await _taskRepository.GetFilteredPersonalTasksForUserAsync(accountId, status, priority, deadline);
        }
        public List<Employee> GetStaffByTeam(string teamId)
        {
            return _employeeRepository.GetStaffByTeam(teamId);
        }

        public List<Subtask> GetSubtasksByTeam(string teamId)
        {
            return _taskRepository.GetSubtasksByTeam(teamId);
        }

        public void AssignSubtask(string subtaskId, string assignedTo, string createdBy)
        {
            _taskRepository.AssignSubtask(subtaskId, assignedTo, createdBy);
        }

        public void UnassignSubtask(string subtaskId)
        {
            _taskRepository.UnassignSubtask(subtaskId);
        }
    }
}
