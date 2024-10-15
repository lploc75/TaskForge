using System;
using System.Collections.Generic;

namespace TaskForge.Models;

public partial class Team
{
    public string TeamId { get; set; } = null!;

    public string? TeamName { get; set; }

    public DateOnly? CreatedDate { get; set; }

    public int? NumberOfMember { get; set; }

    public string? DeptId { get; set; }

    public virtual Department? Dept { get; set; }

    public virtual ICollection<Subtask> Subtasks { get; set; } = new List<Subtask>();

    public virtual ICollection<Employee> Accounts { get; set; } = new List<Employee>();
}
