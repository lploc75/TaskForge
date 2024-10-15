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

    public virtual ICollection<EmployeeSubTask> EmployeeSubTaskAssignedToNavigations { get; set; } = new List<EmployeeSubTask>();

    public virtual ICollection<EmployeeSubTask> EmployeeSubTaskCreatedByNavigations { get; set; } = new List<EmployeeSubTask>();

    public virtual ICollection<EmployeeTask> EmployeeTaskAssignedToNavigations { get; set; } = new List<EmployeeTask>();

    public virtual ICollection<EmployeeTask> EmployeeTaskCreatedByNavigations { get; set; } = new List<EmployeeTask>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();
}
