namespace window_form.Models;

public sealed class MilkSale
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; } = DateTime.Today;
    public decimal QuantityLitres { get; set; }
    public decimal PricePerLitre { get; set; }
    public decimal TotalAmount => QuantityLitres * PricePerLitre;
}
