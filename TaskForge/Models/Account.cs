using System;
using System.Collections.Generic;

namespace TaskForge.Models;

public partial class Account
{
    public string AccountId { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Email { get; set; }

    public string? Role { get; set; }

    public string? PhoneNumber { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual StaffAndLeader? StaffAndLeader { get; set; }
}
