public class InitiatePaymentDto
{
    public string PhoneNumber { get; set; } = string.Empty; // Format: 2547XXXXXXXX
    public decimal Amount { get; set; }
    public string AccountReference { get; set; } = string.Empty; // Usually user email or ID
    public string TransactionDesc { get; set; } = "Farm Management Subscription";
}

public class PaymentResponseDto
{
    public bool Success { get; set; }
    public string? Message { get; set; }
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