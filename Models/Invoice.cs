using System;
using System.Collections.Generic;

namespace PUNDERO.Models;

public partial class Invoice
{
    public int IdInvoice { get; set; }

    public DateTime IssueDate { get; set; }

    public int IdObject { get; set; }

    public int IdWarehouse { get; set; }

    public int IdStatus { get; set; }

    public virtual Object IdObjectNavigation { get; set; } = null!;

    public virtual InvoiceStatus IdStatusNavigation { get; set; } = null!;

    public virtual Warehouse IdWarehouseNavigation { get; set; } = null!;

    public virtual ICollection<InvoiceProduct> InvoiceProducts { get; set; } = new List<InvoiceProduct>();
}
