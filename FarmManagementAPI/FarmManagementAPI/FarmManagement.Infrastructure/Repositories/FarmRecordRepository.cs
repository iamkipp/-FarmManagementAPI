using Microsoft.EntityFrameworkCore;
using FarmManagement.Core.Entities;
using FarmManagement.Core.Interfaces;
using FarmManagement.Infrastructure.Data;

namespace FarmManagement.Infrastructure.Repositories;

public class FarmRecordRepository : IFarmRecordRepository
{
    private readonly ApplicationDbContext _context;

    public FarmRecordRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<FarmRecord>> GetRecordsByFarmIdAsync(Guid farmId)
    {
        return await _context.FarmRecords
            .Where(r => r.FarmId == farmId)
            .OrderByDescending(r => r.RecordDate)
            .ToListAsync();
    }

    public async Task<FarmRecord?> GetRecordByIdAsync(Guid id)
    {
        return await _context.FarmRecords
            .Include(r => r.Farm)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task AddFarmRecordAsync(FarmRecord record)
    {
        await _context.FarmRecords.AddAsync(record);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateFarmRecordAsync(FarmRecord record)
    {
        _context.FarmRecords.Update(record);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteFarmRecordAsync(Guid id)
    {
        var record = await _context.FarmRecords.FindAsync(id);
        if (record != null)
        {
            _context.FarmRecords.Remove(record);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<FarmRecord>> GetRecordsByUserIdAsync(Guid userId)
    {
        return await _context.FarmRecords
            .Include(r => r.Farm)
            .Where(r => r.Farm.UserId == userId)
            .OrderByDescending(r => r.RecordDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<FarmRecord>> GetRecordsByDateRangeAsync(Guid farmId, DateTime startDate, DateTime endDate)
    {
        return await _context.FarmRecords
            .Where(r => r.FarmId == farmId &&
                       r.RecordDate >= startDate &&
                       r.RecordDate <= endDate)
            .OrderBy(r => r.RecordDate)
            .ToListAsync();
    }

    public async Task<int> GetRecordCountByFarmIdAsync(Guid farmId)
    {
        return await _context.FarmRecords
            .Where(r => r.FarmId == farmId)
            .CountAsync();
    }

    public async Task<decimal> GetTotalYieldByFarmIdAsync(Guid farmId)
    {
        return await _context.FarmRecords
            .Where(r => r.FarmId == farmId)
            .SumAsync(r => r.YieldInKg);
    }

    public async Task<decimal> GetTotalRevenueByFarmIdAsync(Guid farmId)
    {
        return await _context.FarmRecords
            .Where(r => r.FarmId == farmId)
            .SumAsync(r => r.Revenue);
    }
}