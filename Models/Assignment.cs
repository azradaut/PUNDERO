using System;
using System.Collections.Generic;

namespace PUNDERO.Models;

public partial class Assignment
{
    public int IdAssignment { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int IdDriver { get; set; }

    public int IdVehicle { get; set; }

    public int IdMobile { get; set; }

    public int IdAssignmentType { get; set; }

    public virtual AssignmentType IdAssignmentTypeNavigation { get; set; } = null!;

    public virtual Driver IdDriverNavigation { get; set; } = null!;

    public virtual Mobile IdMobileNavigation { get; set; } = null!;

    public virtual Vehicle IdVehicleNavigation { get; set; } = null!;
}
