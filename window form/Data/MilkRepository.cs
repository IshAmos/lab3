using Microsoft.Data.Sqlite;
using window_form.Models;

namespace window_form.Data;

public sealed class MilkRepository
{
    private readonly Database _database;

    public MilkRepository(Database database)
    {
        _database = database;
    }

    public List<MilkCollection> GetCollections()
    {
        using var connection = _database.CreateConnection();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, FarmerName, CollectionDate, QuantityLitres, PricePerLitre FROM MilkCollections ORDER BY CollectionDate DESC, Id DESC";

        using var reader = command.ExecuteReader();
        var results = new List<MilkCollection>();
        while (reader.Read())
        {
            results.Add(new MilkCollection
            {
                Id = reader.GetInt32(0),
                FarmerName = reader.GetString(1),
                CollectionDate = DateTime.Parse(reader.GetString(2)),
                QuantityLitres = reader.GetDecimal(3),
                PricePerLitre = reader.GetDecimal(4)
            });
        }

        return results;
    }

    public void AddCollection(MilkCollection collection)
    {
        using var connection = _database.CreateConnection();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO MilkCollections (FarmerName, CollectionDate, QuantityLitres, PricePerLitre) VALUES ($farmer, $date, $quantity, $price)";
        command.Parameters.AddWithValue("$farmer", collection.FarmerName);
        command.Parameters.AddWithValue("$date", collection.CollectionDate.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("$quantity", collection.QuantityLitres);
        command.Parameters.AddWithValue("$price", collection.PricePerLitre);
        command.ExecuteNonQuery();
    }

    public void UpdateCollection(MilkCollection collection)
    {
        using var connection = _database.CreateConnection();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE MilkCollections SET FarmerName = $farmer, CollectionDate = $date, QuantityLitres = $quantity, PricePerLitre = $price WHERE Id = $id";
        command.Parameters.AddWithValue("$id", collection.Id);
        command.Parameters.AddWithValue("$farmer", collection.FarmerName);
        command.Parameters.AddWithValue("$date", collection.CollectionDate.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("$quantity", collection.QuantityLitres);
        command.Parameters.AddWithValue("$price", collection.PricePerLitre);
        command.ExecuteNonQuery();
    }

    public void DeleteCollection(int id)
    {
        ExecuteDelete("MilkCollections", id);
    }

    public List<MilkSale> GetSales()
    {
        using var connection = _database.CreateConnection();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, CustomerName, SaleDate, QuantityLitres, PricePerLitre FROM MilkSales ORDER BY SaleDate DESC, Id DESC";

        using var reader = command.ExecuteReader();
        var results = new List<MilkSale>();
        while (reader.Read())
        {
            results.Add(new MilkSale
            {
                Id = reader.GetInt32(0),
                CustomerName = reader.GetString(1),
                SaleDate = DateTime.Parse(reader.GetString(2)),
                QuantityLitres = reader.GetDecimal(3),
                PricePerLitre = reader.GetDecimal(4)
            });
        }

        return results;
    }

    public void AddSale(MilkSale sale)
    {
        using var connection = _database.CreateConnection();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO MilkSales (CustomerName, SaleDate, QuantityLitres, PricePerLitre) VALUES ($customer, $date, $quantity, $price)";
        command.Parameters.AddWithValue("$customer", sale.CustomerName);
        command.Parameters.AddWithValue("$date", sale.SaleDate.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("$quantity", sale.QuantityLitres);
        command.Parameters.AddWithValue("$price", sale.PricePerLitre);
        command.ExecuteNonQuery();
    }

    public void UpdateSale(MilkSale sale)
    {
        using var connection = _database.CreateConnection();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE MilkSales SET CustomerName = $customer, SaleDate = $date, QuantityLitres = $quantity, PricePerLitre = $price WHERE Id = $id";
        command.Parameters.AddWithValue("$id", sale.Id);
        command.Parameters.AddWithValue("$customer", sale.CustomerName);
        command.Parameters.AddWithValue("$date", sale.SaleDate.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("$quantity", sale.QuantityLitres);
        command.Parameters.AddWithValue("$price", sale.PricePerLitre);
        command.ExecuteNonQuery();
    }

    public void DeleteSale(int id)
    {
        ExecuteDelete("MilkSales", id);
    }

    private void ExecuteDelete(string tableName, int id)
    {
        using var connection = _database.CreateConnection();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = $"DELETE FROM {tableName} WHERE Id = $id";
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
    }
}
