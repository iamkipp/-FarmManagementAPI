using System.ComponentModel.DataAnnotations;

namespace FarmManagement.API.DTOs
{
    public class OutputDto
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string CropType { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal QuantityHarvested { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Revenue { get; set; }

        public DateTime DateRecorded { get; set; }
        public Guid FarmerId { get; set; }
        public string FarmerName { get; set; } = string.Empty;
    }

    public class CreateOutputDto
    {
        [Required]
        [StringLength(100)]
        public string CropType { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal QuantityHarvested { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Revenue { get; set; }
    }

    public class UpdateOutputDto
    {
        [Required]
        [StringLength(100)]
        public string CropType { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal QuantityHarvested { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Revenue { get; set; }
    }
}