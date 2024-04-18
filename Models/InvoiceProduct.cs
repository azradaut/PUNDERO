using System;
using System.Collections.Generic;

namespace PUNDERO.Models;

public partial class InvoiceProduct
{
    public int IdInvoice { get; set; }

    public int IdProduct { get; set; }

    public int OrderQuantity { get; set; }

    public virtual Invoice IdInvoiceNavigation { get; set; } = null!;

    public virtual Product IdProductNavigation { get; set; } = null!;
}
