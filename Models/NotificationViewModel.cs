public class NotificationViewModel
{
    public int IdNotification { get; set; }
    public int IdAccount { get; set; }
    public int AccountType { get; set; }
    public string Message { get; set; } = null!;
    public bool? Seen { get; set; }
    public DateTime? CreatedAt { get; set; }
    public int? IdInvoice { get; set; }
    public int? InvoiceStatusId { get; set; }
}
