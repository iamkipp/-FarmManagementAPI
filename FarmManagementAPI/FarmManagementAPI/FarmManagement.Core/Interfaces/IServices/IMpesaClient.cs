using FarmManagementAPI.FarmManagement.Shared.Dtos;


namespace FarmManagementAPI.FarmManagement.Core.Interfaces;

public interface IMpesaClient
{
    Task<StkPushResponse> StkPushRequestAsync(StkPushRequest request); // ✅ Must return StkPushResponse
    Task<string> GetAccessTokenAsync();
}