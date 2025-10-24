namespace FarmManagement.Core.Interfaces;

public interface INotificationService
{
    Task<bool> SendSmsAsync(string phoneNumber, string message);
    Task<bool> SendEmailAsync(string email, string subject, string message);
    Task<bool> SendSubscriptionReminderAsync(Guid userId, int daysUntilExpiry);
    Task<bool> SendPaymentConfirmationAsync(Guid userId, decimal amount, string receiptNumber);
}