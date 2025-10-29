using FarmManagementAPI.FarmManagement.Core.Entities;

namespace FarmManagementAPI.FarmManagement.Core.Interfaces.Repositories;

public interface ISubscriptionRepository
{
    Task<Subscription?> GetSubscriptionByUserIdAsync(Guid userId);
    Task<Subscription?> GetSubscriptionByIdAsync(Guid id);
    Task UpdateSubscriptionAsync(Subscription subscription);
    Task<IEnumerable<Subscription>> GetExpiringSubscriptionsAsync(int daysBeforeExpiry);
    Task<IEnumerable<Subscription>> GetExpiredSubscriptionsAsync();
    Task<IEnumerable<Subscription>> GetActiveSubscriptionsAsync();
    Task<int> GetActiveSubscriptionCountAsync();
}