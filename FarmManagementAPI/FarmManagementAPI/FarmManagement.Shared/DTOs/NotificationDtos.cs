namespace FarmManagementAPI.FarmManagement.Shared.Dtos;
public class SmsMessageDto
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? SenderId { get; set; }
}

public class EmailMessageDto
{
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsHtml { get; set; } = true;
}

public class NotificationResultDto
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? NotificationId { get; set; }
    public DateTime SentAt { get; set; }
}
