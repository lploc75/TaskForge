using System;
using System.Collections.Generic;

namespace TaskForge.Models;

public partial class Project
{
    public int ProjectId { get; set; }

    public string? ProjectName { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public DateTime? Deadline { get; set; }

    public virtual ICollection<EmployeeProject> EmployeeProjects { get; set; } = new List<EmployeeProject>();

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();
}
