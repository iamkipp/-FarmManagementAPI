public interface IAdminService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<SubscriptionReportDto> GetSubscriptionReportAsync();
    Task<SystemAnalyticsDto> GetSystemAnalyticsAsync();
}