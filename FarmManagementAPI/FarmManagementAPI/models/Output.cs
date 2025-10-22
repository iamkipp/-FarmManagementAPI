using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmManagement.API.Models
{
    public class Output
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid FarmerId { get; set; }

        [Required]
        [StringLength(100)]
        public string CropType { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal QuantityHarvested { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Revenue { get; set; }

        public DateTime DateRecorded { get; set; } = DateTime.UtcNow;

        // Navigation property
        [ForeignKey("FarmerId")]
        public User Farmer { get; set; } = null!;
    }
}
