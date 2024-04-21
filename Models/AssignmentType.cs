using System;
using System.Collections.Generic;

namespace PUNDERO.Models;

public partial class AssignmentType
{
    public int IdAssignmentType { get; set; }

    public string Description { get; set; } = null!;

    public virtual ICollection<MobileDriver> MobileDrivers { get; set; } = new List<MobileDriver>();

    public virtual ICollection<VehicleDriver> VehicleDrivers { get; set; } = new List<VehicleDriver>();
}
