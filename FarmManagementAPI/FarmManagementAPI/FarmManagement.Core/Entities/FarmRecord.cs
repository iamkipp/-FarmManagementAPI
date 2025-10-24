public class FarmRecord
{
    public Guid Id { get; set; }
    public DateTime RecordDate { get; set; }
    public string SeedType { get; set; } = string.Empty;
    public decimal SeedAmountInKg { get; set; }
    public string SoilCondition { get; set; } = string.Empty;
    public string PesticideUsed { get; set; } = string.Empty;
    public decimal PesticideAmountInLitres { get; set; }
    public decimal WaterConsumptionInLitres { get; set; }
    public decimal YieldInKg { get; set; }
    public decimal Revenue { get; set; }
    public string Notes { get; set; } = string.Empty;

    // Foreign key
    public Guid FarmId { get; set; }
    public virtual Farm Farm { get; set; } = null!;
}
