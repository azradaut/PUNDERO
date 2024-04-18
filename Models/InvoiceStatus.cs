using System;
using System.Collections.Generic;

namespace PUNDERO.Models;

public partial class InvoiceStatus
{
    public int IdStatus { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
