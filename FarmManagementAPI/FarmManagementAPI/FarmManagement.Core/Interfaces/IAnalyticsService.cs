public interface IAnalyticsService
{
    Task<FarmSummaryDto> GetFarmSummaryAsync(Guid userId);
    Task<IEnumerable<YieldTrendDto>> GetYieldTrendsAsync(Guid userId, int months);
    Task<EfficiencyMetricsDto> GetEfficiencyMetricsAsync(Guid userId);
}