using System;
using System.Collections.Generic;

namespace TaskForge.Models;

public partial class SubtaskEvaluation
{
    public string EvaluationId { get; set; } = null!;

    public DateOnly? EvaluationDate { get; set; }

    public string? Comment { get; set; }

    public string? SubtaskId { get; set; }

    public int? TeamworkRating { get; set; }

    public int? TimelinessRating { get; set; }

    public int? KpiRating { get; set; }

    public virtual Subtask? Subtask { get; set; }
}
