using System;
using System.Collections.Generic;

namespace PUNDERO.Models;

public partial class WorkingHour
{
    public int IdWorkingHours { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public double Duration { get; set; }

    public virtual ICollection<Tachograph> Tachographs { get; set; } = new List<Tachograph>();
}
