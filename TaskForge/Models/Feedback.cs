using System;
using System.Collections.Generic;

namespace TaskForge.Models;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    public string? Context { get; set; }

    public DateTime DateSubmitted { get; set; }

    public string? AccountId { get; set; }

    public virtual Account? Account { get; set; }
}
