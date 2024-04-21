using System;
using System.Collections.Generic;

namespace PUNDERO.Models;

public partial class Object
{
    public int IdObject { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public double Longitude { get; set; }

    public double Latitude { get; set; }

    public string Qr { get; set; } = null!;

    public int? IdClient { get; set; }

    public virtual Client? IdClientNavigation { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
