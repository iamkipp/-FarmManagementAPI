using FarmManagementAPI.FarmManagement.Shared.Dtos;
using FarmManagementAPI.FarmManagement.Core.Entities;

namespace FarmManagementAPI.FarmManagement.Core.Interfaces.IServices;

public interface IPaymentService
{
    Task<PaymentResponseDto> InitiateStkPushAsync(InitiatePaymentDto paymentDto, Guid userId);
    Task<bool> HandlePaymentCallbackAsync(PaymentCallbackDto callbackDto);
    Task<bool> VerifyPaymentAsync(string checkoutRequestId);
    Task<IEnumerable<Payment>> GetUserPaymentHistoryAsync(Guid userId);
    Task<bool> IsPaymentPendingAsync(Guid userId);
}