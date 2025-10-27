namespace FarmManagementAPI.FarmManagement.Core.Entities;

public class Subscription
{
    public Guid Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsTrial { get; set; } = true;
    public string Status { get; set; } = "Active";

    // ========== ADD THESE PROPERTIES ==========

    // Payment tracking
    public string? LastMpesaReceiptNumber { get; set; }
    public DateTime? LastPaymentDate { get; set; }
    public decimal? LastPaymentAmount { get; set; }
    public string? LastPaymentPhoneNumber { get; set; }

    // Track payment attempts
    public string? PendingCheckoutRequestId { get; set; }
    public DateTime? PendingPaymentDate { get; set; }

    // ==========================================

    // Foreign key
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
}