using FarmManagementAPI.models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FarmManagement.API.Models
{
    public class User : IdentityUser<Guid>
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Role { get; set; } = "Farmer";

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Input> Inputs { get; set; } = new List<Input>();
        public ICollection<Output> Outputs { get; set; } = new List<Output>();
    }

    public enum UserRole
    {
        Admin,
        Farmer
    }
}