using System;
using System.Collections.Generic;

namespace TaskForge.Models;

public partial class Credit
{
    public int Difficulty { get; set; }

    public int? Credits { get; set; }

    public virtual ICollection<Subtask> Subtasks { get; set; } = new List<Subtask>();
}
