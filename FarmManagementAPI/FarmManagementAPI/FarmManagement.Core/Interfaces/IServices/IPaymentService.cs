using FarmManagement.Shared.Dtos;
using FarmManagement.Core.Entities;

namespace FarmManagement.Core.Interfaces;

public interface IPaymentService
{
    Task<PaymentResponseDto> InitiateStkPushAsync(InitiatePaymentDto paymentDto, Guid userId);
    Task<bool> HandlePaymentCallbackAsync(PaymentCallbackDto callbackDto);
    Task<bool> VerifyPaymentAsync(string checkoutRequestId);
    Task<IEnumerable<Payment>> GetUserPaymentHistoryAsync(Guid userId);
    Task<bool> IsPaymentPendingAsync(Guid userId);
}