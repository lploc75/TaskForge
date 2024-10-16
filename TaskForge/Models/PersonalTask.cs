using System;
using System.Collections.Generic;

namespace TaskForge.Models;

public partial class PersonalTask
{
    public string PtaskId { get; set; } = null!;

    public string? AccountId { get; set; }

    public string? Status { get; set; }

    public int? Priority { get; set; }

    public DateTime? AssignmentDate { get; set; }

    public DateOnly? Deadline { get; set; }

    public string? Description { get; set; }

    public virtual StaffAndLeader? Account { get; set; }
}
