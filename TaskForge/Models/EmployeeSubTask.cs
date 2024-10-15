using System;
using System.Collections.Generic;

namespace TaskForge.Models;

public partial class EmployeeSubTask
{
    public string SubtaskId { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    public string AssignedTo { get; set; } = null!;

    public virtual Employee AssignedToNavigation { get; set; } = null!;

    public virtual Employee CreatedByNavigation { get; set; } = null!;

    public virtual Subtask Subtask { get; set; } = null!;
}
