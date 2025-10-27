namespace FarmManagementAPI.FarmManagement.Shared.Dtos;
public class RevenueReportDto
{
    public decimal TotalRevenue { get; set; }
    public decimal AverageRevenue { get; set; }
    public decimal HighestRevenue { get; set; }
    public decimal LowestRevenue { get; set; }
    public IEnumerable<MonthlyRevenueDetailDto> MonthlyDetails { get; set; } = new List<MonthlyRevenueDetailDto>();
}

public class MonthlyRevenueDetailDto
{
    public string Month { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public decimal Yield { get; set; }
    public int TransactionCount { get; set; }
}

public class CropPerformanceDto
{
    public string CropType { get; set; } = string.Empty;
    public decimal TotalYield { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageYield { get; set; }
    public decimal AverageRevenue { get; set; }
    public int FarmCount { get; set; }
}

public class ExportDataDto
{
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public byte[] Data { get; set; } = Array.Empty<byte>();
    public long FileSize { get; set; }
}