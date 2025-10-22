using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmManagement.API.Models
{
    public class Input
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid FarmerId { get; set; }

        [Required]
        [StringLength(50)]
        public string InputType { get; set; } = string.Empty; // Seed, Fertilizer, Insecticide

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Cost { get; set; }

        public DateTime DateAdded { get; set; } = DateTime.UtcNow;

        // Navigation property
        [ForeignKey("FarmerId")]
        public User Farmer { get; set; } = null!;
    }

    public enum InputType
    {
        Seed,
        Fertilizer,
        Insecticide
    }
}