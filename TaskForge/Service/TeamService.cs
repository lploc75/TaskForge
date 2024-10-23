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

        // Tạo đội nhóm mới
        public void CreateTeam(Team team)
        {
            _teamRepository.CreateTeam(team);
        }

        // Chỉnh sửa đội nhóm
        public void UpdateTeam(Team team)
        {
            _teamRepository.UpdateTeam(team);
        }

        // Xóa đội nhóm
        public void DeleteTeam(string teamId)
        {
            _teamRepository.DeleteTeam(teamId);
        }
    }
}
