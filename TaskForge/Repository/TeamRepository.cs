using System.Collections.Generic;
using System.Linq;
using TaskForge.DBContext;
using TaskForge.Models;

namespace TaskForge.Repository
{
    public class TeamRepository
    {
        private readonly TaskForgeContext _context;

        public TeamRepository(TaskForgeContext context)
        {
            _context = context;
        }

        public List<Team> GetTeamsByDepartment(string deptId)
        {
            return _context.Teams.Where(t => t.DeptId == deptId).ToList();
        }
    }
}
