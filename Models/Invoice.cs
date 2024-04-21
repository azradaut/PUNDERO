using System;
using System.Collections.Generic;

namespace PUNDERO.Models;

public partial class Invoice
{
    public int IdInvoice { get; set; }

    public DateTime IssueDate { get; set; }

    public int? IdObject { get; set; }

    public int? IdWarehouse { get; set; }

    public int? IdStatus { get; set; }

    public virtual Object? IdObjectNavigation { get; set; }

    public virtual InvoiceStatus? IdStatusNavigation { get; set; }

    public virtual Warehouse? IdWarehouseNavigation { get; set; }

    public virtual ICollection<InvoiceProduct> InvoiceProducts { get; set; } = new List<InvoiceProduct>();
}
