using System;
using System.Collections.Generic;

namespace TaskForge.Models;

public partial class TaskEvaluation
{
    public string EvaluationId { get; set; } = null!;

    public DateOnly? EvaluationDate { get; set; }

    public string? Comment { get; set; }

    public string? TaskId { get; set; }

    public virtual Task? Task { get; set; }
}
