using window_form.Data;
using window_form.Models;

namespace window_form.Controllers;

public sealed class SalesController
{
    private readonly MilkRepository _repository;

    public SalesController(MilkRepository repository)
    {
        _repository = repository;
    }

    public List<MilkSale> GetAll() => _repository.GetSales();

    public void Save(MilkSale sale)
    {
        Validate(sale.CustomerName, sale.QuantityLitres, sale.PricePerLitre);
        if (sale.Id == 0)
        {
            _repository.AddSale(sale);
        }
        else
        {
            _repository.UpdateSale(sale);
        }
    }

    public void Delete(int id)
    {
        if (id > 0)
        {
            _repository.DeleteSale(id);
        }
    }

    private static void Validate(string customerName, decimal quantity, decimal price)
    {
        if (string.IsNullOrWhiteSpace(customerName))
        {
            throw new ArgumentException("Enter the customer name.");
        }

        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than zero.");
        }

        if (price < 0)
        {
            throw new ArgumentException("Price cannot be negative.");
        }
    }
}
