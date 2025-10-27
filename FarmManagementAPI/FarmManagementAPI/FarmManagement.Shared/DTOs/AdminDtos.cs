namespace FarmManagementAPI.FarmManagement.Shared.Dtos;
public class SystemAnalyticsDto
{
    public int TotalUsers { get; set; }
    public int TotalFarms { get; set; }
    public int TotalRecords { get; set; }
    public int ActiveSubscriptions { get; set; }
    public int TrialUsers { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal MonthlyRecurringRevenue { get; set; }
    public IEnumerable<UserGrowthDto> UserGrowth { get; set; } = [];
}

public class SubscriptionReportDto
{
    public int TotalSubscriptions { get; set; }
    public int ActiveSubscriptions { get; set; }
    public int ExpiredSubscriptions { get; set; }
    public int TrialSubscriptions { get; set; }
    public decimal MonthlyRecurringRevenue { get; set; }
    public IEnumerable<SubscriptionStatsDto> SubscriptionStats { get; set; } = new List<SubscriptionStatsDto>();
}

public class RevenueSummaryDto
{
    public decimal TotalRevenue { get; set; }
    public decimal ThisMonthRevenue { get; set; }
    public decimal LastMonthRevenue { get; set; }
    public decimal AverageRevenuePerUser { get; set; }
    public IEnumerable<MonthlyRevenueDto> MonthlyRevenue { get; set; } = new List<MonthlyRevenueDto>();
}

public class UserGrowthDto
{
    public string Period { get; set; } = string.Empty;
    public int NewUsers { get; set; }
    public int TotalUsers { get; set; }
}

public class SubscriptionStatsDto
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Percentage { get; set; }
}

public class MonthlyRevenueDto
{
    public string Month { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public int TransactionCount { get; set; }
}
