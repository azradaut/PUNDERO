using System;
using System.Collections.Generic;

namespace PUNDERO.Models;

public partial class Product
{
    public int IdProduct { get; set; }

    public string NameProduct { get; set; } = null!;

    public int Quantity { get; set; }

    public double Price { get; set; }

    public int Barcode { get; set; }

    public int? IdWarehouse { get; set; }

    public virtual Warehouse? IdWarehouseNavigation { get; set; }

    public virtual ICollection<InvoiceProduct> InvoiceProducts { get; set; } = new List<InvoiceProduct>();
}
