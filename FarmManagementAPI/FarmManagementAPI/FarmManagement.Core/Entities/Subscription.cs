public class Subscription
{
    public Guid Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsTrial { get; set; } = true;
    public string Status { get; set; } = "Active"; // Active, Expired, Cancelled

    // Foreign key
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;

    // Payment info
    public string? MpesaReceiptNumber { get; set; }
    public DateTime? LastPaymentDate { get; set; }
}
