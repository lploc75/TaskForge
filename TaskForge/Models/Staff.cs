using System;
using System.Collections.Generic;

namespace TaskForge.Models;

public partial class Staff
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

    public decimal? TotalKpi { get; set; }

    public decimal? TotalTimeliness { get; set; }

    public decimal? TotalTeamwork { get; set; }

    public int? CreditPoints { get; set; }

    public int? NumberOfTeam { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<CreditExchange> CreditExchanges { get; set; } = new List<CreditExchange>();

    public virtual Department? Dept { get; set; }
}
