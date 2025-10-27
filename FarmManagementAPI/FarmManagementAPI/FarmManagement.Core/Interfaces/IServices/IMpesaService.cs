namespace FarmManagementAPI.FarmManagement.Core.Interfaces.IServices;

public interface IMpesaService
{
    Task<bool> ValidatePhoneNumberAsync(string phoneNumber);
    Task<decimal> CalculateSubscriptionFeeAsync(Guid userId);
    Task<string> GenerateAccountReferenceAsync(Guid userId);
    Task<bool> IsBusinessShortCodeValidAsync(string businessShortCode);
}