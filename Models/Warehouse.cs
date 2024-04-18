using System;
using System.Collections.Generic;

namespace PUNDERO.Models;

public partial class Warehouse
{
    public int IdWarehouse { get; set; }

    public string NameWarehouse { get; set; } = null!;

    public string Address { get; set; } = null!;

    public double Longitude { get; set; }

    public double Latitude { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
