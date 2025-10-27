using FarmManagementAPI.FarmManagement.Core.Entities;

namespace FarmManagementAPI.FarmManagement.Core.Interfaces.IServices;

public interface ITokenService
{
    string CreateToken(User user);
}