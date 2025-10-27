
using FarmManagementAPI.FarmManagement.Shared.Dtos;
namespace FarmManagementAPI.FarmManagement.Core.Interfaces.IServices;

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