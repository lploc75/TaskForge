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

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

    public virtual ICollection<Employee> Accounts { get; set; } = new List<Employee>();

    public virtual ICollection<Department> Depts { get; set; } = new List<Department>();
}
