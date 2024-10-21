using System;
using System.Collections.Generic;

namespace TaskForge.Models;

public partial class File
{
    public string FileId { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public DateOnly UploadDate { get; set; }

    public string FilePath { get; set; } = null!;

    public string? AccountId { get; set; }

    public string? SubtaskId { get; set; }

    public virtual StaffAndLeader? Account { get; set; }

    public virtual Subtask? Subtask { get; set; }
}
