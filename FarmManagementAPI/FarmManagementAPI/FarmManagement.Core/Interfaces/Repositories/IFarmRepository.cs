using FarmManagementAPI.FarmManagement.Core.Entities;

namespace FarmManagementAPI.FarmManagement.Core.Interfaces.Repositories;

public interface IFarmRepository
{
    Task<IEnumerable<Farm>> GetFarmsByUserIdAsync(Guid userId);
    Task<Farm?> GetFarmByIdAsync(Guid id);
    Task AddFarmAsync(Farm farm);
    Task UpdateFarmAsync(Farm farm);
    Task DeleteFarmAsync(Guid id);
    Task<int> GetFarmCountByUserIdAsync(Guid userId);
    Task<Farm?> GetFarmWithRecordsAsync(Guid farmId);
}