namespace FarmManagementAPI.FarmManagement.Core.Entities
{
    public class Payment
    {
        public string? LastMpesaReceiptNumber { get; set; }
        public DateTime? LastPaymentDate { get; set; }
        public decimal? LastPaymentAmount { get; set; }
        public string? LastPaymentPhoneNumber { get; set; }
    }
}
