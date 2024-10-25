using System.Collections.Generic;
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

        public List<Team> GetTeamsByDepartment(string deptId)
        {
            return _teamRepository.GetTeamsByDepartment(deptId);
        }
    }
}
