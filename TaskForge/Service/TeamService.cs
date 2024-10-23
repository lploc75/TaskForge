using TaskForge.Models;
using TaskForge.Repository;

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

        // Phương thức lấy danh sách team với các bộ lọc
        public List<Team> GetTeamsWithFilters(string deptId, int? numberOfTeam, DateOnly? createdDate)
        {
            return _teamRepository.GetTeamsWithFilters(deptId, numberOfTeam, createdDate);
        }
    }
}
