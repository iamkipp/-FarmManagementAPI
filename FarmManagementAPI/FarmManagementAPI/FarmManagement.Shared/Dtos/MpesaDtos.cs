namespace FarmManagementAPI.FarmManagement.Shared.Dtos;

public class StkPushRequest
{
    public string BusinessShortCode { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Timestamp { get; set; } = string.Empty;
    public string TransactionType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string PartyA { get; set; } = string.Empty;
    public string PartyB { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string CallBackURL { get; set; } = string.Empty;
    public string AccountReference { get; set; } = string.Empty;
    public string TransactionDesc { get; set; } = string.Empty;
}

public class StkPushResponse
{
    public string MerchantRequestID { get; set; } = string.Empty;
    public string CheckoutRequestID { get; set; } = string.Empty;
    public string ResponseCode { get; set; } = string.Empty;
    public string ResponseDescription { get; set; } = string.Empty;
    public string CustomerMessage { get; set; } = string.Empty;
}