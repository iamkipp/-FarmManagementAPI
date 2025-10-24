using FarmManagement.Core.Entities;

namespace FarmManagement.Core.Interfaces;

public interface ITokenService
{
    string CreateToken(User user);
}