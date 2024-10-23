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
        // Phương thức trong TeamRepository
        public Team GetTeamById(string teamId)
        {
            // Tìm và trả về team theo teamId
            return _context.Teams.FirstOrDefault(t => t.TeamId == teamId);
        }
        // Phương thức để đếm số lượng team hiện có
        public int GetTeamCount()
        {
            return _context.Teams.Count();
        }
        // Thêm team mới vào DB
        public void AddTeam(Team team)
        {
            _context.Teams.Add(team);
            _context.SaveChanges();
        }

        // Cập nhật team trong DB
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
        public List<Team> GetTeamsWithFilters(string deptId, int? numberOfTeam, DateOnly? createdDate)
        {
            // Lấy tất cả team từ cơ sở dữ liệu
            var query = _context.Teams.AsQueryable();

            // Lọc theo department nếu có
            if (!string.IsNullOrEmpty(deptId))
            {
                query = query.Where(t => t.DeptId == deptId);
            }

            // Lọc theo số nhóm nếu có
            if (numberOfTeam.HasValue)
            {
                query = query.Where(t => t.NumberOfMember >= numberOfTeam);
            }

            // Lấy danh sách team từ database
            var teams = query.ToList();

            // Sau khi lấy dữ liệu từ database, lọc theo ngày tạo bằng cách chuyển đổi DateOnly sang DateTime
            if (createdDate.HasValue)
            {
                teams = teams.Where(t => t.CreatedDate.HasValue && t.CreatedDate.Value >= createdDate.Value).ToList();
            }

            return teams;
        }

    }
}
