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

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    public virtual Account IdAccountNavigation { get; set; } = null!;

    public virtual Tachograph IdTachographNavigation { get; set; } = null!;

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<Mobile> Mobiles { get; set; } = new List<Mobile>();

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
