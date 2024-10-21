using System;
using System.Collections.Generic;

namespace TaskForge.Models;

public partial class EmployeeProject
{
    public string AccountId { get; set; } = null!;

    public int ProjectId { get; set; }

    public string? Role { get; set; }

    public virtual Employee Account { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;
}
