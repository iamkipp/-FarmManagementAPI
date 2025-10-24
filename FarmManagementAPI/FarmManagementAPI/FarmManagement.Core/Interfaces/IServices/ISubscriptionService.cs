using FarmManagement.Shared.Dtos;

namespace FarmManagement.Core.Interfaces;

public interface ISubscriptionService
{
    Task<SubscriptionStatusDto> GetSubscriptionStatusAsync(Guid userId);
    Task<bool> CheckAndUpdateSubscriptionAsync(Guid userId);
    Task CancelSubscriptionAsync(Guid userId);
    Task<bool> HasActiveSubscriptionAsync(Guid userId);
    Task<bool> CanAccessFeatureAsync(Guid userId, string feature);
    Task<DateTime?> GetSubscriptionEndDateAsync(Guid userId);
}
