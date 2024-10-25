using System;
using System.Collections.Generic;

namespace TaskForge.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public string? AccountId { get; set; }

    public string? Message { get; set; }

    public DateTime? CreatedDate { get; set; }

    public bool? IsRead { get; set; }

    public virtual StaffAndLeader? Account { get; set; }
}
