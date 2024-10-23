using TaskForge.Models;
using TaskForge.DBContext;
using System.Collections.Generic;
using System.Linq;

namespace TaskForge.Repository
{
    public class TeamRepository
    {
        private readonly TaskForgeContext _context;

        public TeamRepository(TaskForgeContext context)
        {
            _context = context;
        }

        // Lấy tất cả các đội nhóm
        public List<Team> GetAllTeams()
        {
            return _context.Teams.ToList();
        }

        // Tạo đội nhóm mới
        public void CreateTeam(Team team)
        {
            _context.Teams.Add(team);
            _context.SaveChanges();
        }

        // Cập nhật đội nhóm
        public void UpdateTeam(Team team)
        {
            _context.Teams.Update(team);
            _context.SaveChanges();
        }

        // Xóa đội nhóm
        public void DeleteTeam(string teamId)
        {
            var team = _context.Teams.FirstOrDefault(t => t.TeamId == teamId);
            if (team != null)
            {
                _context.Teams.Remove(team);
                _context.SaveChanges();
            }
        }
    }
}
