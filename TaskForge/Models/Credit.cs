using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskForge.Models;

public partial class Credit
{
    [Key]
    public int Difficulty { get; set; }

    public int? Credits { get; set; }

    public virtual ICollection<Subtask> Subtasks { get; set; } = new List<Subtask>();
}
