using TaskForge.Models;
using TaskForge.DBContext;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

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
            string sql = "SELECT * FROM Team WHERE team_id = @team_id";

            return _context.Teams
                .FromSqlRaw(sql, new SqlParameter("@team_id", teamId))
                .AsEnumerable()
                .FirstOrDefault();
        }

        // Phương thức để đếm số lượng team hiện có
        public int GetTeamCount()
        {
            return _context.Teams.Count();
        }
        // Thêm team mới vào DB
        public void AddTeam(Team team)
        {
            string sql = @"
        INSERT INTO Team (team_id, team_name, created_date, number_of_member, dept_id) 
        VALUES (@team_id, @team_name, @created_date, @number_of_member, @dept_id)";

            _context.Database.ExecuteSqlRaw(sql,
                new SqlParameter("@team_id", team.TeamId),
                new SqlParameter("@team_name", team.TeamName),
                new SqlParameter("@created_date", team.CreatedDate),
                new SqlParameter("@number_of_member", team.NumberOfMember),
                new SqlParameter("@dept_id", team.DeptId)
            );
        }

        public void UpdateTeam(Team team)
        {
            string sql = @"
        UPDATE Team 
        SET 
            team_name = @team_name, 
            created_date = @created_date, 
            number_of_member = @number_of_member, 
            dept_id = @dept_id 
        WHERE team_id = @team_id";

            _context.Database.ExecuteSqlRaw(sql,
                new SqlParameter("@team_id", team.TeamId),
                new SqlParameter("@team_name", team.TeamName),
                new SqlParameter("@created_date", team.CreatedDate),
                new SqlParameter("@number_of_member", team.NumberOfMember),
                new SqlParameter("@dept_id", team.DeptId)
            );
        }


        // Xóa đội nhóm
        public void DeleteTeam(string teamId)
        {
            string sql = "DELETE FROM Team WHERE team_id = @team_id";

            _context.Database.ExecuteSqlRaw(sql, new SqlParameter("@team_id", teamId));
        }

        public List<Team> GetTeamsWithFilters(string deptId, int? numberOfTeamFrom, int? numberOfTeamTo, DateOnly? createdDateFrom, DateOnly? createdDateTo)
        {
            string sql = "SELECT * FROM Team WHERE 1=1";
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(deptId))
            {
                sql += " AND dept_id = @deptId";
                parameters.Add(new SqlParameter("@deptId", deptId));
            }

            if (numberOfTeamFrom.HasValue)
            {
                sql += " AND number_of_member >= @numberOfTeamFrom";
                parameters.Add(new SqlParameter("@numberOfTeamFrom", numberOfTeamFrom));
            }

            if (numberOfTeamTo.HasValue)
            {
                sql += " AND number_of_member <= @numberOfTeamTo";
                parameters.Add(new SqlParameter("@numberOfTeamTo", numberOfTeamTo));
            }

            if (createdDateFrom.HasValue)
            {
                sql += " AND created_date >= @createdDateFrom";
                parameters.Add(new SqlParameter("@createdDateFrom", createdDateFrom.Value.ToDateTime(TimeOnly.MinValue)));
            }

            if (createdDateTo.HasValue)
            {
                sql += " AND created_date <= @createdDateTo";
                parameters.Add(new SqlParameter("@createdDateTo", createdDateTo.Value.ToDateTime(TimeOnly.MinValue)));
            }

            return _context.Teams.FromSqlRaw(sql, parameters.ToArray()).ToList();
        }
        public void AddMemberToTeam(string teamId, string accountId)
        {
            string sql = "INSERT INTO EmployeeTeam (team_id, account_id) VALUES (@teamId, @accountId)";
            _context.Database.ExecuteSqlRaw(sql,
                new SqlParameter("@teamId", teamId),
                new SqlParameter("@accountId", accountId));
        }

        public void RemoveMemberFromTeam(string teamId, string accountId)
        {
            string sql = "DELETE FROM EmployeeTeam WHERE team_id = @teamId AND account_id = @accountId";
            _context.Database.ExecuteSqlRaw(sql,
                new SqlParameter("@teamId", teamId),
                new SqlParameter("@accountId", accountId));
        }

        public List<Team> GetTeamsByDepartment(string deptId)
        {
            return _context.Teams.Where(t => t.DeptId == deptId).ToList();
        }
        public List<Team> GetTeamsByTaskSubtasks(string taskId)
        {
            return _context.Teams
                           .Where(t => t.Subtasks.Any(st => st.TaskId == taskId))
                           .ToList();
        }
    }
}
