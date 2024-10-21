using System;
using System.Collections.Generic;

namespace TaskForge.Models;

public partial class DepartmentTask
{
    public string TaskId { get; set; } = null!;

    public string DeptId { get; set; } = null!;

    public int? DeptParticipantCount { get; set; }

    public string? AdditonalDept { get; set; }

    public virtual Department Dept { get; set; } = null!;

    public virtual Task Task { get; set; } = null!;
}
