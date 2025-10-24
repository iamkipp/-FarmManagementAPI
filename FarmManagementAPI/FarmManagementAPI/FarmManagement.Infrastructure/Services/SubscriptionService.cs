using Microsoft.EntityFrameworkCore;
using FarmManagement.Core.Interfaces;
using FarmManagement.Shared.Dtos;
using FarmManagement.Core.Entities;
using FarmManagement.Infrastructure.Data;

namespace FarmManagement.Infrastructure.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SubscriptionService> _logger;

    public SubscriptionService(ApplicationDbContext context, ILogger<SubscriptionService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<SubscriptionStatusDto> GetSubscriptionStatusAsync(Guid userId)
    {
        var user = await _context.Users
            .Include(u => u.Subscription)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user?.Subscription == null)
            throw new Exception("Subscription not found");

        var subscription = user.Subscription;
        var daysRemaining = subscription.EndDate.HasValue
            ? (int)(subscription.EndDate.Value - DateTime.UtcNow).TotalDays
            : 0;

        var status = new SubscriptionStatusDto
        {
            IsActive = subscription.IsActive,
            IsTrial = subscription.IsTrial,
            TrialEndDate = subscription.IsTrial ? subscription.EndDate : null,
            SubscriptionEndDate = subscription.IsTrial ? null : subscription.EndDate,
            Status = subscription.Status,
            DaysRemaining = Math.Max(0, daysRemaining),
            LastPaymentDate = subscription.LastPaymentDate,
            LastPaymentAmount = subscription.LastPaymentAmount,
            HasPendingPayment = !string.IsNullOrEmpty(subscription.PendingCheckoutRequestId)
        };

        // Auto-update status if expired
        if (subscription.EndDate.HasValue && subscription.EndDate < DateTime.UtcNow && subscription.IsActive)
        {
            subscription.IsActive = false;
            subscription.Status = "Expired";
            await _context.SaveChangesAsync();

            status.IsActive = false;
            status.Status = "Expired";
            status.DaysRemaining = 0;
        }

        return status;
    }

    public async Task<bool> CheckAndUpdateSubscriptionAsync(Guid userId)
    {
        var user = await _context.Users
            .Include(u => u.Subscription)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user?.Subscription == null) return false;

        var subscription = user.Subscription;

        // Check if subscription has expired
        if (subscription.EndDate.HasValue && subscription.EndDate < DateTime.UtcNow && subscription.IsActive)
        {
            subscription.IsActive = false;
            subscription.Status = "Expired";
            await _context.SaveChangesAsync();
            return false;
        }

        return subscription.IsActive;
    }

    public async Task CancelSubscriptionAsync(Guid userId)
    {
        var user = await _context.Users
            .Include(u => u.Subscription)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user?.Subscription != null)
        {
            user.Subscription.IsActive = false;
            user.Subscription.Status = "Cancelled";
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Subscription cancelled for user {user.Email}");
        }
    }

    public async Task<bool> HasActiveSubscriptionAsync(Guid userId)
    {
        var user = await _context.Users
            .Include(u => u.Subscription)
            .FirstOrDefaultAsync(u => u.Id == userId);

        return user?.Subscription?.IsActive == true &&
               user.Subscription.EndDate.HasValue &&
               user.Subscription.EndDate > DateTime.UtcNow;
    }

    public async Task<bool> CanAccessFeatureAsync(Guid userId, string feature)
    {
        // Basic feature access control based on subscription status
        var hasActiveSubscription = await HasActiveSubscriptionAsync(userId);

        return feature.ToLower() switch
        {
            "analytics" => hasActiveSubscription,
            "export" => hasActiveSubscription,
            "multiple-farms" => hasActiveSubscription,
            _ => true // Basic features are available to all
        };
    }

    public async Task<DateTime?> GetSubscriptionEndDateAsync(Guid userId)
    {
        var user = await _context.Users
            .Include(u => u.Subscription)
            .FirstOrDefaultAsync(u => u.Id == userId);

        return user?.Subscription?.EndDate;
    }
}