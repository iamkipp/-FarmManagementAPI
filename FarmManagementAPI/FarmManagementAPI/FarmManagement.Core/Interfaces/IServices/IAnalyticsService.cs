﻿using FarmManagement.Shared.Dtos;

namespace FarmManagement.Core.Interfaces;

public interface IAnalyticsService
{
    Task<FarmSummaryDto> GetFarmSummaryAsync(Guid userId);
    Task<IEnumerable<YieldTrendDto>> GetYieldTrendsAsync(Guid userId, int months = 6);
    Task<EfficiencyMetricsDto> GetEfficiencyMetricsAsync(Guid userId);
    Task<IEnumerable<ResourceUsageDto>> GetResourceUsageAsync(Guid userId);
    Task<RevenueReportDto> GetRevenueReportAsync(Guid userId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<CropPerformanceDto>> GetCropPerformanceAsync(Guid userId);
}