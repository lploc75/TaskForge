using System;
using System.Collections.Generic;

namespace TaskForge.Models;

public partial class StaffAndLeader
{
    public string AccountId { get; set; } = null!;

    public decimal? TotalKpi { get; set; }

    public decimal? TotalTimeliness { get; set; }

    public decimal? TotalTeamwork { get; set; }

    public int? CreditPoints { get; set; }

    public int? NumberOfTeam { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<CreditExchange> CreditExchanges { get; set; } = new List<CreditExchange>();

    public virtual ICollection<PersonalTask> PersonalTasks { get; set; } = new List<PersonalTask>();

    public virtual ICollection<SubtaskAssignment> SubtaskAssignmentAssignedToNavigations { get; set; } = new List<SubtaskAssignment>();

    public virtual ICollection<SubtaskAssignment> SubtaskAssignmentCreatedByNavigations { get; set; } = new List<SubtaskAssignment>();
}
