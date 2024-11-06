    using System.Collections.Generic;
using System.Security;
using TaskForge.Models;
    using TaskForge.Repository;
using X.PagedList;
using X.PagedList.Extensions;

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
        public bool CreatePersonalTask(string PtaskName, DateTime assignedDate, DateTime deadline, int priority, string description, string accountId)
        {

            // Sinh ID ngẫu nhiên cho PersonalTask
            string ptaskId = GenerateRandomPtaskId();

            // Tạo đối tượng PersonalTask mới
            var ptask = new PersonalTask
            {
                PtaskId = ptaskId,
                AccountId = accountId,
                PtaskName = PtaskName,
                Status = "In Progress",
                Priority = priority,
                AssignmentDate = assignedDate,
                Deadline = deadline,
                Description = description
            };

            // Gọi repository để thêm PersonalTask vào cơ sở dữ liệu
            _taskRepository.AddPersonalTask(ptask);

            return true; // Thành công
        }

        private string GenerateRandomPtaskId()
        {
            Random random = new Random();
            int randomNumber = random.Next(1, 1000);
            return "PT" + randomNumber.ToString("D3");
        }


        // Lấy PersonalTask theo Id
        public PersonalTask GetPersonalTaskById(string PtaskId)
        {
            return _taskRepository.GetPersonalTaskById(PtaskId);
        }

        // Cập nhật thông tin của Personal Task
        public bool UpdatePersonalTask(string PtaskId, string PtaskName, string Description, DateTime assignedDate, DateTime deadline, int priority)
        {
            // Lấy PersonalTask từ repository
            var personalTask = _taskRepository.GetPersonalTaskById(PtaskId);
            if (personalTask == null)
            {
                return false; // Task không tồn tại
            }

            // Cập nhật thông tin của PersonalTask
            personalTask.PtaskName = PtaskName;
            personalTask.Description = Description;
            personalTask.AssignmentDate = assignedDate;
            personalTask.Deadline = deadline;
            personalTask.Priority = priority;

            // Gọi repository để cập nhật PersonalTask trong cơ sở dữ liệu
            _taskRepository.UpdatePersonalTask(personalTask);

            return true; // Cập nhật thành công
        }

        public bool DeletePersonalTask(string ptaskId)
        {
            // Kiểm tra xem task có tồn tại không
            var ptask = _taskRepository.GetPersonalTaskById(ptaskId);
            if (ptask == null)
            {
                return false;
            }

            // Xóa task nếu tồn tại
            _taskRepository.DeletePersonalTask(ptaskId);
            return true;
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
        public List<TaskForge.Models.Task> GetTasksByDepartment(string deptId)
        {
            return _taskRepository.GetTasksByDepartmentId(deptId);
        }
        // Lọc cho danh sách nhiệm vụ cá nhân
        public PagedList<PersonalTask> GetFilteredPersonalTasks(string status, int? priority, DateTime? assignmentDateMin, DateTime? assignmentDateMax, DateTime? deadlineMin, DateTime? deadlineMax, int page, int pageSize)
        {
            var personalTasks = _taskRepository.FilterPersonalTasks(status, priority, assignmentDateMin, assignmentDateMax, deadlineMin, deadlineMax);

            // Phân trang logic
            var pagedPersonalTasks = personalTasks.ToPagedList(page, pageSize);
            return (PagedList<PersonalTask>)pagedPersonalTasks;
        }
        public bool UpdatePersonalTaskStatus(string PtaskId, string currentStatus)
        {
            // Lấy PersonalTask từ repository
            var Ptask = _taskRepository.GetPersonalTaskById(PtaskId);

            if (Ptask != null)
            {
                // Thay đổi trạng thái
                Ptask.Status = currentStatus.Equals("In Progress") ? "Completed" : "In Progress";

                // Cập nhật PersonalTask
                _taskRepository.UpdatePtask(Ptask);
                return true;
            }
            return false;
        }
        public List<SubtaskAssignment> GetAllSubtaskAssignments()
        {
            return _taskRepository.GetAllSubtaskAssignments();
        }
        public IPagedList<Subtask> GetFilteredSubtasks(string teamId, string subtaskId, string subtaskName, string status,
                                                  int? priority, int? difficulty, DateTime? startDate, DateTime? endDate,
                                                  int pageNumber, int pageSize)
        {
            // Lấy danh sách subtasks từ Repository
            var subtasks = _taskRepository.GetSubtasksByTeam(teamId).AsQueryable();

            // Áp dụng các điều kiện lọc
            if (!string.IsNullOrEmpty(subtaskId))
            {
                subtasks = subtasks.Where(s => s.SubtaskId.Contains(subtaskId));
            }
            if (!string.IsNullOrEmpty(subtaskName))
            {
                subtasks = subtasks.Where(s => s.SubtaskName.Contains(subtaskName));
            }
            if (!string.IsNullOrEmpty(status))
            {
                subtasks = subtasks.Where(s => s.Status == status);
            }
            if (priority.HasValue)
            {
                subtasks = subtasks.Where(s => s.Priority == priority);
            }
            if (difficulty.HasValue)
            {
                subtasks = subtasks.Where(s => s.Difficulty == difficulty);
            }
            if (startDate.HasValue)
            {
                subtasks = subtasks.Where(s => s.AssignmentDate >= startDate);
            }
            if (endDate.HasValue)
            {
                subtasks = subtasks.Where(s => s.Deadline <= endDate);
            }

            // Áp dụng phân trang
            return subtasks.ToPagedList(pageNumber, pageSize);
        }
    }
}
