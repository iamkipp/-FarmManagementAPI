using System.ComponentModel.DataAnnotations;

namespace FarmManagementAPI.FarmManagement.Core.Entities;
public class Farm
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;

    // Change from double to decimal for consistency
    public decimal SizeInAcres { get; set; }

    public string SoilType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign key
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;

    // Navigation properties
    public virtual ICollection<FarmRecord> FarmRecords { get; set; } = new List<FarmRecord>();
}