namespace FarmManagement.API.DTOs
{
    public class ReportSummaryDto
    {
        public Guid FarmerId { get; set; }
        public string FarmerName { get; set; } = string.Empty;
        public decimal TotalInputCost { get; set; }
        public decimal TotalOutputRevenue { get; set; }
        public decimal Profit { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }
}