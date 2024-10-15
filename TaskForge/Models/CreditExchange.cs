using System;
using System.Collections.Generic;

namespace TaskForge.Models;

public partial class CreditExchange
{
    public int ExchangeId { get; set; }

    public string? AccountId { get; set; }

    public DateOnly ExchangeDate { get; set; }

    public int CreditPointsUsed { get; set; }

    public decimal CashAmount { get; set; }

    public string? Status { get; set; }

    public virtual Staff? Account { get; set; }
}
