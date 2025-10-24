using FarmManagement.Core.Entities;

namespace FarmManagement.Core.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> GetUserByIdAsync(Guid id);
    Task AddUserAsync(User user);
    Task<bool> UserExistsAsync(string email);
    Task UpdateUserAsync(User user);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<IEnumerable<User>> GetUsersByRoleAsync(string role);
}
