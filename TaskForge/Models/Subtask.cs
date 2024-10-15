using System;
using System.Collections.Generic;

namespace TaskForge.Models;

public partial class Subtask
{
    public string SubtaskId { get; set; } = null!;

    public string? SubtaskName { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public int? Priority { get; set; }

    public int? Difficulty { get; set; }

    public DateOnly? Deadline { get; set; }

    public DateOnly? SubmissionDate { get; set; }

    public string? TaskId { get; set; }

    public string? TeamId { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<EmployeeSubTask> EmployeeSubTasks { get; set; } = new List<EmployeeSubTask>();

    public virtual ICollection<SubtaskEvaluation> SubtaskEvaluations { get; set; } = new List<SubtaskEvaluation>();

    public virtual Task? Task { get; set; }

    public virtual Team? Team { get; set; }

    public virtual ICollection<Credit> Difficulties { get; set; } = new List<Credit>();
}
