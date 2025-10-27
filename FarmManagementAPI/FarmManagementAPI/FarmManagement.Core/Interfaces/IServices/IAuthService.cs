
using FarmManagementAPI.FarmManagement.Shared.Dtos;

namespace FarmManagementAPI.FarmManagement.Core.Interfaces.IServices;

public interface IAuthService
{
    Task<UserDto> RegisterAsync(RegisterDto registerDto);
    Task<string> LoginAsync(LoginDto loginDto);
    Task<UserDto> GetCurrentUserAsync(string email);
    Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
}