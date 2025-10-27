namespace FarmManagementAPI.FarmManagement.Shared.Dtos
{
    public class SubsriptionStatusDtos
    {
        public bool IsActive { get; set; }
        public bool IsTrial { get; set; }
        public DateTime? TrialEndDate { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int DaysRemaining { get; set; }
        public DateTime? LastPaymentDate { get; set; }
        public decimal? LastPaymentAmount { get; set; }
        public bool HasPendingPayment { get; set; }
    }
}
