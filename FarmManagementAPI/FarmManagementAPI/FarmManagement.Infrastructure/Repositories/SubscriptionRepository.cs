using Microsoft.EntityFrameworkCore;
using FarmManagementAPI.FarmManagement.Core.Entities;
using FarmManagement.Core.Interfaces;
using FarmManagementAPI.FarmManagement.Infrastructure.Data;

namespace FarmManagementAPI.FarmManagement.Infrastructure.Repositories;

public class SubscriptionRepository(ApplicationDbContext context) : ISubscriptionRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Subscription?> GetSubscriptionByUserIdAsync(Guid userId)
    {
        return await _context.Subscriptions
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.UserId == userId);
    }

    public async Task<Subscription?> GetSubscriptionByIdAsync(Guid id)
    {
        return await _context.Subscriptions
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task UpdateSubscriptionAsync(Subscription subscription)
    {
        _context.Subscriptions.Update(subscription);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Subscription>> GetExpiringSubscriptionsAsync(int daysBeforeExpiry)
    {
        var targetDate = DateTime.UtcNow.AddDays(daysBeforeExpiry);
        return await _context.Subscriptions
            .Include(s => s.User)
            .Where(s => s.IsActive &&
                       s.EndDate.HasValue &&
                       s.EndDate.Value <= targetDate &&
                       s.EndDate.Value > DateTime.UtcNow)
            .ToListAsync();
    }

    public async Task<IEnumerable<Subscription>> GetExpiredSubscriptionsAsync()
    {
        return await _context.Subscriptions
            .Include(s => s.User)
            .Where(s => s.IsActive &&
                       s.EndDate.HasValue &&
                       s.EndDate.Value < DateTime.UtcNow)
            .ToListAsync();
    }

    public async Task<IEnumerable<Subscription>> GetActiveSubscriptionsAsync()
    {
        return await _context.Subscriptions
            .Include(s => s.User)
            .Where(s => s.IsActive &&
                       (!s.EndDate.HasValue || s.EndDate.Value > DateTime.UtcNow))
            .ToListAsync();
    }

    public async Task<int> GetActiveSubscriptionCountAsync()
    {
        return await _context.Subscriptions
            .CountAsync(s => s.IsActive &&
                           (!s.EndDate.HasValue || s.EndDate.Value > DateTime.UtcNow));
    }
}