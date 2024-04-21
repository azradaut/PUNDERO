using System;
using System.Collections.Generic;

namespace PUNDERO.Models;

public partial class Driver
{
    public int IdDriver { get; set; }

    public int PrivateMobile { get; set; }

    public string LicenseNumber { get; set; } = null!;

    public string LicenseCategory { get; set; } = null!;

    public int IdTachograph { get; set; }

    public int IdAccount { get; set; }

    public virtual Account IdAccountNavigation { get; set; } = null!;

    public virtual Tachograph IdTachographNavigation { get; set; } = null!;

    public virtual ICollection<MobileDriver> MobileDrivers { get; set; } = new List<MobileDriver>();

    public virtual ICollection<VehicleDriver> VehicleDrivers { get; set; } = new List<VehicleDriver>();
}
