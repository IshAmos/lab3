namespace window_form.Models;

public sealed class MilkCollection
{
    public int Id { get; set; }
    public string FarmerName { get; set; } = string.Empty;
    public DateTime CollectionDate { get; set; } = DateTime.Today;
    public decimal QuantityLitres { get; set; }
    public decimal PricePerLitre { get; set; }
    public decimal TotalAmount => QuantityLitres * PricePerLitre;
}
