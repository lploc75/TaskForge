using System;
using System.Collections.Generic;

namespace TaskForge.Models;

public partial class Department
{
    public string DeptId { get; set; } = null!;

    public string? DeptName { get; set; }

    public string? Description { get; set; }

    public int? NumberOfTeam { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
