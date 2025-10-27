namespace FarmManagementAPI.FarmManagement.Shared.Dtos;
public class InitiatePaymentDto
{
    public string PhoneNumber { get; set; } = string.Empty; // Format: 2547XXXXXXXX
    public decimal Amount { get; set; } = 500; // Fixed monthly fee
    public string AccountReference { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class PaymentResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? CheckoutRequestId { get; set; }
    public string? MerchantRequestId { get; set; }
    public string? CustomerMessage { get; set; }
}

public class PaymentCallbackDto
{
    public string CheckoutRequestID { get; set; } = string.Empty;
    public string MerchantRequestID { get; set; } = string.Empty;
    public int ResultCode { get; set; }
    public string ResultDesc { get; set; } = string.Empty;
    public decimal? Amount { get; set; }
    public string? MpesaReceiptNumber { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? TransactionDate { get; set; }
}

public class PaymentStatusDto
{
    public string CheckoutRequestId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // Pending, Completed, Failed, Cancelled
    public string? MpesaReceiptNumber { get; set; }
    public DateTime? TransactionDate { get; set; }
    public string? ErrorMessage { get; set; }
}

public class PaymentHistoryDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? MpesaReceiptNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
}