using System.ComponentModel.DataAnnotations;

namespace FarmManagement.API.DTOs
{
    public class InputDto
    {
        public Guid Id { get; set; }

        [Required]
        public string InputType { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Cost { get; set; }

        public DateTime DateAdded { get; set; }
        public Guid FarmerId { get; set; }
        public string FarmerName { get; set; } = string.Empty;
    }

    public class CreateInputDto
    {
        [Required]
        public string InputType { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Cost { get; set; }
    }

    public class UpdateInputDto
    {
        [Required]
        public string InputType { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Cost { get; set; }
    }
}
