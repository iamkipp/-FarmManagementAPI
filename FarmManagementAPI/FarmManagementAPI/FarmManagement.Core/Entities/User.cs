using FarmManagementAPI.FarmManagement.Core.Entities;
public class User
{
    public Guid Id { get; set; }
public string Email { get; set; } = string.Empty;
public string FirstName { get; set; } = string.Empty;
public string LastName { get; set; } = string.Empty;
public string PhoneNumber { get; set; } = string.Empty;
public byte[] PasswordHash { get; set; } = new byte[32];
public byte[] PasswordSalt { get; set; } = new byte[32];
public string Role { get; set; } = "Farmer";
public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

// Navigation properties
public virtual ICollection<Farm> Farms { get; set; } = new List<Farm>();
public virtual Subscription Subscription { get; set; } = new Subscription();
}