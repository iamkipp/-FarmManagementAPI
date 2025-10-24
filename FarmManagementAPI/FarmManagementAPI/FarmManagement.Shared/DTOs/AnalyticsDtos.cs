public class FarmSummaryDto
{
    public int TotalFarms { get; set; }
    public decimal TotalYield { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageYieldPerFarm { get; set; }
    public IEnumerable<FarmYieldDto> FarmYields { get; set; } = new List<FarmYieldDto>();
}

public class FarmYieldDto
{
    public string FarmName { get; set; } = string.Empty;
    public decimal TotalYield { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageYieldPerAcre { get; set; }
}

public class YieldTrendDto
{
    public string Period { get; set; } = string.Empty;
    public decimal Yield { get; set; }
    public decimal Revenue { get; set; }
    public int RecordCount { get; set; }
}

public class EfficiencyMetricsDto
{
    public decimal YieldToSeedRatio { get; set; }
    public decimal RevenuePerAcre { get; set; }
    public decimal WaterEfficiency { get; set; }
    public int TotalFarms { get; set; }
    public int TotalRecords { get; set; }
}

public class ResourceUsageDto
{
    public string FarmName { get; set; } = string.Empty;
    public decimal TotalWaterUsed { get; set; }
    public decimal TotalSeedUsed { get; set; }
    public decimal TotalPesticideUsed { get; set; }
    public decimal AverageYield { get; set; }
}

public class SubscriptionStatusDto
{
    public bool IsActive { get; set; }
    public bool IsTrial { get; set; }
    public DateTime? TrialEndDate { get; set; }
    public DateTime? SubscriptionEndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public int DaysRemaining { get; set; }
    public DateTime? LastPaymentDate { get; set; }
    public decimal? LastPaymentAmount { get; set; }
}