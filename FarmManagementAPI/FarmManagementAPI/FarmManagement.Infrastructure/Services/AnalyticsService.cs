using FarmManagementAPI.FarmManagement.Infrastructure.Data;
using FarmManagementAPI.FarmManagement.Shared.Dtos;
using FarmManagementAPI.FarmManagement.Core.Interfaces.IServices;
using Microsoft.EntityFrameworkCore;

namespace FarmManagementAPI.FarmManagement.Infrastructure.Services;

public class AnalyticsService(ApplicationDbContext context, ILogger<AnalyticsService> logger) : IAnalyticsService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<AnalyticsService> _logger = logger;

    public async Task<FarmSummaryDto> GetFarmSummaryAsync(Guid userId)
    {
        var farms = await _context.Farms
            .Include(f => f.FarmRecords)
            .Where(f => f.UserId == userId)
            .ToListAsync();

        var totalYield = farms.Sum(f => f.FarmRecords.Sum(r => r.YieldInKg));
        var totalRevenue = farms.Sum(f => f.FarmRecords.Sum(r => r.Revenue));
        var totalFarms = farms.Count;

        var farmYields = farms.Select(f => new FarmYieldDto
        {
            FarmName = f.Name,
            TotalYield = f.FarmRecords.Sum(r => r.YieldInKg),
            TotalRevenue = f.FarmRecords.Sum(r => r.Revenue),
            AverageYieldPerAcre = f.SizeInAcres > 0 ? f.FarmRecords.Sum(r => r.YieldInKg) / f.SizeInAcres : 0
        }).ToList();

        return new FarmSummaryDto
        {
            TotalFarms = totalFarms,
            TotalYield = totalYield,
            TotalRevenue = totalRevenue,
            AverageYieldPerFarm = totalFarms > 0 ? totalYield / totalFarms : 0,
            FarmYields = farmYields
        };
    }

    public async Task<IEnumerable<YieldTrendDto>> GetYieldTrendsAsync(Guid userId, int months = 6)
    {
        var startDate = DateTime.UtcNow.AddMonths(-months);

        var records = await _context.FarmRecords
            .Include(r => r.Farm)
            .Where(r => r.Farm.UserId == userId && r.RecordDate >= startDate)
            .OrderBy(r => r.RecordDate)
            .ToListAsync();

        var trends = records
            .GroupBy(r => new { r.RecordDate.Year, r.RecordDate.Month })
            .Select(g => new YieldTrendDto
            {
                Period = $"{g.Key.Year}-{g.Key.Month:D2}",
                Yield = g.Sum(r => r.YieldInKg),
                Revenue = g.Sum(r => r.Revenue),
                RecordCount = g.Count()
            })
            .ToList();

        return trends;
    }

    public async Task<EfficiencyMetricsDto> GetEfficiencyMetricsAsync(Guid userId)
    {
        var records = await _context.FarmRecords
            .Include(r => r.Farm)
            .Where(r => r.Farm.UserId == userId)
            .ToListAsync();

        var totalYield = records.Sum(r => r.YieldInKg);
        var totalSeed = records.Sum(r => r.SeedAmountInKg);
        var totalWater = records.Sum(r => r.WaterConsumptionInLitres);
        var totalRevenue = records.Sum(r => r.Revenue);
        var totalAcres = await _context.Farms
            .Where(f => f.UserId == userId)
            .SumAsync(f => f.SizeInAcres);

        return new EfficiencyMetricsDto
        {
            YieldToSeedRatio = totalSeed > 0 ? totalYield / totalSeed : 0,
            RevenuePerAcre = totalAcres > 0 ? totalRevenue / totalAcres : 0,
            WaterEfficiency = totalWater > 0 ? totalYield / totalWater : 0,
            TotalFarms = await _context.Farms.CountAsync(f => f.UserId == userId),
            TotalRecords = records.Count
        };
    }

    public async Task<IEnumerable<ResourceUsageDto>> GetResourceUsageAsync(Guid userId)
    {
        var records = await _context.FarmRecords
            .Include(r => r.Farm)
            .Where(r => r.Farm.UserId == userId)
            .ToListAsync();

        var usage = records
            .GroupBy(r => r.Farm.Name)
            .Select(g => new ResourceUsageDto
            {
                FarmName = g.Key,
                TotalWaterUsed = g.Sum(r => r.WaterConsumptionInLitres),
                TotalSeedUsed = g.Sum(r => r.SeedAmountInKg),
                TotalPesticideUsed = g.Sum(r => r.PesticideAmountInLitres),
                AverageYield = g.Average(r => r.YieldInKg)
            })
            .ToList();

        return usage;
    }

    public Task<RevenueReportDto> GetRevenueReportAsync(Guid userId, DateTime startDate, DateTime endDate)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<CropPerformanceDto>> GetCropPerformanceAsync(Guid userId)
    {
        throw new NotImplementedException();
    }
}