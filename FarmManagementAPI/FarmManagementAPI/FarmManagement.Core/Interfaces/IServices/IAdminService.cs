using FarmManagement.Shared.Dtos;

namespace FarmManagement.Core.Interfaces;

public interface IAdminService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<SystemAnalyticsDto> GetSystemAnalyticsAsync();
    Task<SubscriptionReportDto> GetSubscriptionReportAsync();
    Task<RevenueSummaryDto> GetRevenueSummaryAsync();
    Task<bool> UpdateUserRoleAsync(Guid userId, string newRole);
    Task<bool> DeactivateUserAsync(Guid userId);
    Task<bool> ExtendTrialAsync(Guid userId, int additionalDays);
}