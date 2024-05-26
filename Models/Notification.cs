using System;
using System.Collections.Generic;

namespace PUNDERO.Models;

public partial class Notification
{
    public int IdNotification { get; set; }

    public int? IdAccount { get; set; }

    public string Message { get; set; } = null!;

    public bool? Seen { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? IdInvoice { get; set; }

    public virtual Account? IdAccountNavigation { get; set; }

    public virtual Invoice? IdInvoiceNavigation { get; set; }
}
