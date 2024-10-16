using System;
using System.Collections.Generic;

namespace TaskForge.Models;

public partial class Employee
{
    public string AccountId { get; set; } = null!;

    public string Fullname { get; set; } = null!;

    public string? Gender { get; set; }

    public DateOnly? Dob { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? Role { get; set; }

    public string? Status { get; set; }

    public string? DeptId { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Department? Dept { get; set; }

    public virtual ICollection<TaskAssignment> TaskAssignmentAssignedToNavigations { get; set; } = new List<TaskAssignment>();

    public virtual ICollection<TaskAssignment> TaskAssignmentCreatedByNavigations { get; set; } = new List<TaskAssignment>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();
}
