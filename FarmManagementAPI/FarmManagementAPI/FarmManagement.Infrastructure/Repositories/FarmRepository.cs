using Microsoft.EntityFrameworkCore;
using FarmManagementAPI.FarmManagement.Core.Entities;
using FarmManagementAPI.FarmManagement.Infrastructure.Data;
using FarmManagementAPI.FarmManagement.Core.Interfaces.Repositories;

namespace FarmManagementAPI.FarmManagement.Infrastructure.Repositories;

public class FarmRepository(ApplicationDbContext context) : IFarmRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<Farm>> GetFarmsByUserIdAsync(Guid userId)
    {
        return await _context.Farms
            .Include(f => f.FarmRecords)
            .Where(f => f.UserId == userId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();
    }

    public async Task<Farm?> GetFarmByIdAsync(Guid id)
    {
        return await _context.Farms
            .Include(f => f.FarmRecords)
            .Include(f => f.User)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<Farm?> GetFarmWithRecordsAsync(Guid farmId)
    {
        return await _context.Farms
            .Include(f => f.FarmRecords)
            .FirstOrDefaultAsync(f => f.Id == farmId);
    }

    public async Task AddFarmAsync(Farm farm)
    {
        await _context.Farms.AddAsync(farm);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateFarmAsync(Farm farm)
    {
        _context.Farms.Update(farm);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteFarmAsync(Guid id)
    {
        var farm = await _context.Farms.FindAsync(id);
        if (farm != null)
        {
            _context.Farms.Remove(farm);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> GetFarmCountByUserIdAsync(Guid userId)
    {
        return await _context.Farms
            .Where(f => f.UserId == userId)
            .CountAsync();
    }
}