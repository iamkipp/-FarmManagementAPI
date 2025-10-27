using FarmManagementAPI.FarmManagement.Core.Entities;

namespace FarmManagementAPI.FarmManagement.Core.Interfaces.Repositories
{
    public interface IFarmRecordRepository
    {
        Task<IEnumerable<FarmRecord>> GetRecordsByFarmIdAsync(Guid farmId);
        Task<FarmRecord?> GetRecordByIdAsync(Guid id);
        Task AddFarmRecordAsync(FarmRecord record);
        Task UpdateFarmRecordAsync(FarmRecord record);
        Task DeleteFarmRecordAsync(Guid id);
        Task<IEnumerable<FarmRecord>> GetRecordsByUserIdAsync(Guid userId);
        Task<IEnumerable<FarmRecord>> GetRecordsByDateRangeAsync(Guid farmId, DateTime startDate, DateTime endDate);
        Task<int> GetRecordCountByFarmIdAsync(Guid farmId);
        Task<decimal> GetTotalYieldByFarmIdAsync(Guid farmId);
        Task<decimal> GetTotalRevenueByFarmIdAsync(Guid farmId);
    }
}