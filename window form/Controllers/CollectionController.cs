using window_form.Data;
using window_form.Models;

namespace window_form.Controllers;

public sealed class CollectionController
{
    private readonly MilkRepository _repository;

    public CollectionController(MilkRepository repository)
    {
        _repository = repository;
    }

    public List<MilkCollection> GetAll() => _repository.GetCollections();

    public void Save(MilkCollection collection)
    {
        Validate(collection.FarmerName, collection.QuantityLitres, collection.PricePerLitre);
        if (collection.Id == 0)
        {
            _repository.AddCollection(collection);
        }
        else
        {
            _repository.UpdateCollection(collection);
        }
    }

    public void Delete(int id)
    {
        if (id > 0)
        {
            _repository.DeleteCollection(id);
        }
    }

    private static void Validate(string farmerName, decimal quantity, decimal price)
    {
        if (string.IsNullOrWhiteSpace(farmerName))
        {
            throw new ArgumentException("Enter the farmer name.");
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
