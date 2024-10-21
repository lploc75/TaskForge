using System;
using System.Collections.Generic;

namespace TaskForge.Models;

public partial class Comment
{
    public string CommentId { get; set; } = null!;

    public string? Content { get; set; }

    public DateTime? DateSubmitted { get; set; }

    public string? SubtaskId { get; set; }

    public virtual Subtask? Subtask { get; set; }
}
