using System;
using System.Collections.Generic;

namespace TaskForge.Models;

public partial class EmployeeTask
{
    public string TaskId { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    public string AssignedTo { get; set; } = null!;

    public virtual Account AssignedToNavigation { get; set; } = null!;

    public virtual Account CreatedByNavigation { get; set; } = null!;
}
