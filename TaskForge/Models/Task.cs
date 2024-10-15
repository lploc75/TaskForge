using System;
using System.Collections.Generic;

namespace TaskForge.Models;

public partial class Task
{
    public string TaskId { get; set; } = null!;

    public string? TaskName { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public int? Priority { get; set; }

    public DateOnly? Deadline { get; set; }

    public DateOnly? SubmissionDate { get; set; }

    public int? ProjectId { get; set; }

    public virtual Project? Project { get; set; }

    public virtual ICollection<Subtask> Subtasks { get; set; } = new List<Subtask>();

    public virtual ICollection<TaskAssignment> TaskAssignments { get; set; } = new List<TaskAssignment>();

    public virtual ICollection<TaskEvaluation> TaskEvaluations { get; set; } = new List<TaskEvaluation>();
}
