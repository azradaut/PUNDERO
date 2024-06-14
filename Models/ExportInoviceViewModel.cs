using System;
using System.Collections.Generic;

namespace PUNDERO.Models
{
    public class ExportInvoiceViewModel
    {
        public int IdInvoice { get; set; }
        public DateTime IssueDate { get; set; }
        public string StoreName { get; set; }
        public string StoreAddress { get; set; }
        public string StorePhone { get; set; } = "+387-33-456-780"; // Mockup broj
        public string StoreEmail { get; set; }
        public List<InvoiceProductViewModel> Products { get; set; } = new List<InvoiceProductViewModel>();
        public double Subtotal { get; set; }
        public double Tax { get; set; }
        public double TotalAmount { get; set; }
        public string DriverName { get; set; }
        public string DriverEmail { get; set; }
        public string DriverPhone { get; set; }
        public string Note { get; set; }

        public class InvoiceProductViewModel
        {
            public string NameProduct { get; set; }
            public int OrderQuantity { get; set; }
            public double UnitPrice { get; set; }
            public double TotalPrice { get; set; }
        }
    }
}
