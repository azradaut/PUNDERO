using System;
using System.Collections.Generic;

namespace PUNDERO.Models;

public partial class Tachograph
{
    public int IdTachograph { get; set; }

    public string Label { get; set; } = null!;

    public DateTime? IssueDate { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public int? IdWorkingHours { get; set; }

    public virtual ICollection<Driver> Drivers { get; set; } = new List<Driver>();

    public virtual WorkingHour? IdWorkingHoursNavigation { get; set; }
}
