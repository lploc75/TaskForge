using TaskForge.Models;
using TaskForge.Repository;
using X.PagedList.Extensions;
using X.PagedList;

namespace TaskForge.Service
{
    public class TeamService
    {
        private readonly TeamRepository _teamRepository;

        public TeamService(TeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }

        // Lấy tất cả các đội nhóm
        public List<Team> GetAllTeams()
        {
            return _teamRepository.GetAllTeams();
        }

        // Tạo mới team
        public void CreateTeam(string TeamName, DateOnly CreatedDate, int NumberOfMember, string DeptId)
        {
            // Tạo TeamId mới
            var teamId = GenerateTeamId();

            // Tạo đối tượng Team mới
            var team = new Team
            {
                TeamId = teamId,
                TeamName = TeamName,
                CreatedDate = CreatedDate,
                NumberOfMember = NumberOfMember,
                DeptId = DeptId
            };

            // Gọi repository để lưu team mới
            _teamRepository.AddTeam(team);
        }
        // Phương thức để tạo TeamId duy nhất
        public string GenerateTeamId()
        {
            // Lấy tổng số team hiện có
            var teamCount = _teamRepository.GetTeamCount();

            // Tạo TeamId mới bằng cách lấy số lượng hiện có và cộng thêm 1
            return "TEAM" + (teamCount + 1).ToString("D3"); // Ví dụ: TEAM001, TEAM002
        }

        // Chỉnh sửa team
        public void EditTeam(string teamId, string TeamName, DateOnly CreatedDate, int NumberOfMember, string DeptId)
        {
            // Lấy team từ repository
            var team = _teamRepository.GetTeamById(teamId);

            if (team != null)
            {
                // Cập nhật thông tin team
                team.TeamName = TeamName;
                team.CreatedDate = CreatedDate;
                team.NumberOfMember = NumberOfMember;
                team.DeptId = DeptId;

                // Gọi repository để lưu thay đổi
                _teamRepository.UpdateTeam(team);
            }
        }
        // Xóa đội nhóm
        public void DeleteTeam(string teamId)
        {
            _teamRepository.DeleteTeam(teamId);
        }
        public List<Team> GetTeamsByDepartment(string deptId)
        {
            return _teamRepository.GetTeamsByDepartment(deptId);
        }
        public List<Team> GetTeamsByTaskSubtasks(string taskId)
        {
            return _teamRepository.GetTeamsByTaskSubtasks(taskId);
        }
        // Phương thức lấy danh sách team với các bộ lọc
        public PagedList<Team> GetTeamsWithFilters(string deptId, int? numberOfTeamFrom, int? numberOfTeamTo, DateOnly? createdDateFrom, DateOnly? createdDateTo, int page, int pageSize)
        {
            // Gọi repository để lấy danh sách team đã lọc
            var teams = _teamRepository.GetTeamsWithFilters(deptId, numberOfTeamFrom, numberOfTeamTo, createdDateFrom, createdDateTo);

            // Thực hiện phân trang
            return (PagedList<Team>)teams.ToPagedList(page, pageSize);
        }
        public void AddMemberToTeam(string teamId, string accountId)
        {
            // Logic kiểm tra trước khi thêm nếu cần
            _teamRepository.AddMemberToTeam(teamId, accountId);
        }

        public void RemoveMemberFromTeam(string teamId, string accountId)
        {
            // Logic kiểm tra trước khi xóa nếu cần
            _teamRepository.RemoveMemberFromTeam(teamId, accountId);
        }
    }
}
