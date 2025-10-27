using FarmManagementAPI.FarmManagement.Core.Entities;
using FarmManagement.Core.Interfaces;
using FarmManagementAPI.FarmManagement.Core.Interfaces.IServices;
using FarmManagementAPI.FarmManagement.Infrastructure.Data;
using FarmManagementAPI.FarmManagement.Shared.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using FarmManagementAPI.FarmManagement.Core.Interfaces;

namespace FarmManagement.Infrastructure.Services;

public class MpesaPaymentService : IPaymentService
{
    private readonly IMpesaClient _mpesaClient;
    private readonly IConfiguration _config;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MpesaPaymentService> _logger;

    public MpesaPaymentService(
        IMpesaClient mpesaClient,
        IConfiguration config,
        ApplicationDbContext context,
        ILogger<MpesaPaymentService> logger)
    {
        _mpesaClient = mpesaClient;
        _config = config;
        _context = context;
        _logger = logger;
    }

    public async Task<PaymentResponseDto> InitiateStkPushAsync(InitiatePaymentDto paymentDto, Guid userId)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.Subscription)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user?.Subscription == null)
                return new PaymentResponseDto { Success = false, Message = "User or subscription not found" };

            // Validate phone number
            if (!paymentDto.PhoneNumber.StartsWith("254") || paymentDto.PhoneNumber.Length != 12)
                return new PaymentResponseDto { Success = false, Message = "Phone number must be in format 254XXXXXXXXX" };

            // Check pending payment
            if (!string.IsNullOrEmpty(user.Subscription.PendingCheckoutRequestId))
                return new PaymentResponseDto { Success = false, Message = "You already have a pending payment" };

            var amount = paymentDto.Amount > 0 ? paymentDto.Amount : 500;

            // Create STK push request
            var stkRequest = new StkPushRequest
            {
                BusinessShortCode = _config["Mpesa:BusinessShortCode"]!,
                Amount = amount,
                PartyA = paymentDto.PhoneNumber,
                PartyB = _config["Mpesa:BusinessShortCode"]!,
                PhoneNumber = paymentDto.PhoneNumber,
                CallBackURL = $"{_config["BaseUrl"]}/api/payments/callback",
                AccountReference = string.IsNullOrEmpty(paymentDto.AccountReference) ? user.Email : paymentDto.AccountReference,
                TransactionDesc = "Farm Management Subscription"
            };

            var response = await _mpesaClient.StkPushRequestAsync(stkRequest);

            if (response.ResponseCode == "0")
            {
                user.Subscription.PendingCheckoutRequestId = response.CheckoutRequestID;
                user.Subscription.PendingPaymentDate = DateTime.UtcNow;
                user.Subscription.LastPaymentPhoneNumber = paymentDto.PhoneNumber;

                await _context.SaveChangesAsync();

                return new PaymentResponseDto
                {
                    Success = true,
                    Message = "Payment initiated successfully. Check your phone to complete the payment.",
                    CheckoutRequestId = response.CheckoutRequestID,
                    MerchantRequestId = response.MerchantRequestID,
                    CustomerMessage = response.CustomerMessage
                };
            }
            else
            {
                return new PaymentResponseDto
                {
                    Success = false,
                    Message = response.ResponseDescription
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initiating M-Pesa payment");
            return new PaymentResponseDto
            {
                Success = false,
                Message = "Failed to initiate payment. Please try again."
            };
        }
    }

    public async Task<bool> HandlePaymentCallbackAsync(PaymentCallbackDto callbackDto)
    {
        try
        {
            _logger.LogInformation($"Received payment callback: {System.Text.Json.JsonSerializer.Serialize(callbackDto)}");

            // Find user by pending checkout request ID
            var user = await _context.Users
                .Include(u => u.Subscription)
                .FirstOrDefaultAsync(u => u.Subscription.PendingCheckoutRequestId == callbackDto.CheckoutRequestID);

            if (user?.Subscription == null)
            {
                _logger.LogWarning($"User not found for CheckoutRequestID: {callbackDto.CheckoutRequestID}");
                return false;
            }

            if (callbackDto.ResultCode == 0) // Payment successful
            {
                // Calculate new end date (extend by 30 days from now or current end date)
                var currentEndDate = user.Subscription.EndDate.HasValue &&
                                   user.Subscription.EndDate > DateTime.UtcNow
                    ? user.Subscription.EndDate.Value
                    : DateTime.UtcNow;

                var newEndDate = currentEndDate.AddDays(30);

                user.Subscription.EndDate = newEndDate;
                user.Subscription.IsTrial = false;
                user.Subscription.IsActive = true;
                user.Subscription.Status = "Active";
                user.Subscription.LastMpesaReceiptNumber = callbackDto.MpesaReceiptNumber;
                user.Subscription.LastPaymentDate = DateTime.UtcNow;
                user.Subscription.LastPaymentAmount = callbackDto.Amount ?? 500;

                // Clear pending payment
                user.Subscription.PendingCheckoutRequestId = null;
                user.Subscription.PendingPaymentDate = null;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Subscription extended for user {user.Email} until {newEndDate}. Receipt: {callbackDto.MpesaReceiptNumber}");
                return true;
            }
            else
            {
                // Payment failed - clear pending payment
                user.Subscription.PendingCheckoutRequestId = null;
                user.Subscription.PendingPaymentDate = null;
                await _context.SaveChangesAsync();

                _logger.LogWarning($"Payment failed for user {user.Email}: {callbackDto.ResultDesc}");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling payment callback");
            return false;
        }
    }

    public async Task<bool> VerifyPaymentAsync(string checkoutRequestId)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.Subscription)
                .FirstOrDefaultAsync(u => u.Subscription.PendingCheckoutRequestId == checkoutRequestId);

            // If no pending payment found and subscription is active, payment was successful
            if (user?.Subscription != null &&
                user.Subscription.PendingCheckoutRequestId == null &&
                user.Subscription.IsActive)
            {
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying payment");
            return false;
        }
    }

    public async Task<bool> IsPaymentPendingAsync(Guid userId)
    {
        var user = await _context.Users
            .Include(u => u.Subscription)
            .FirstOrDefaultAsync(u => u.Id == userId);

        return user?.Subscription?.PendingCheckoutRequestId != null;
    }

    public async Task<IEnumerable<PaymentHistoryDto>> GetUserPaymentHistoryAsync(Guid userId)
    {
        var user = await _context.Users
            .Include(u => u.Subscription)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user?.Subscription == null)
            return Enumerable.Empty<PaymentHistoryDto>();

        var paymentHistory = new List<PaymentHistoryDto>();

        // Add subscription payment history
        if (user.Subscription.LastPaymentDate.HasValue)
        {
            paymentHistory.Add(new PaymentHistoryDto
            {
                Id = Guid.NewGuid(),
                Amount = user.Subscription.LastPaymentAmount ?? 500,
                Status = "Completed",
                MpesaReceiptNumber = user.Subscription.LastMpesaReceiptNumber,
                CreatedAt = user.Subscription.LastPaymentDate.Value,
                CompletedAt = user.Subscription.LastPaymentDate.Value,
                PhoneNumber = user.Subscription.LastPaymentPhoneNumber ?? user.PhoneNumber
            });
        }

        return paymentHistory.OrderByDescending(p => p.CreatedAt);
    }

    Task<IEnumerable<Payment>> IPaymentService.GetUserPaymentHistoryAsync(Guid userId)
    {
        throw new NotImplementedException();
    }
}