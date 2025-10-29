using FarmManagementAPI.FarmManagement.Shared.Dtos;
using System.Threading.Tasks;

namespace FarmManagementAPI.FarmManagement.Core.Interfaces.IServices
{
    public interface IMpesaClient
    {
        Task<string> GetAccessTokenAsync();
        Task<StkPushResponse> StkPushRequestAsync(StkPushRequest request);
    }
}
