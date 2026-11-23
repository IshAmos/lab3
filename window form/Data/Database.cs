using Microsoft.Data.Sqlite;

namespace window_form.Data;

public sealed class Database
{
    private readonly string _connectionString;

    public string DatabasePath { get; }

    public Database()
    {
        var dataDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "MilkManagement");
        Directory.CreateDirectory(dataDirectory);
        DatabasePath = Path.Combine(dataDirectory, "milk-management.db");
        _connectionString = $"Data Source={DatabasePath}";
    }

    public SqliteConnection CreateConnection() => new(_connectionString);

    public void Initialize()
    {
        using var connection = CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS MilkCollections (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                FarmerName TEXT NOT NULL,
                CollectionDate TEXT NOT NULL,
                QuantityLitres REAL NOT NULL,
                PricePerLitre REAL NOT NULL
            );

            CREATE TABLE IF NOT EXISTS MilkSales (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CustomerName TEXT NOT NULL,
                SaleDate TEXT NOT NULL,
                QuantityLitres REAL NOT NULL,
                PricePerLitre REAL NOT NULL
            );
            """;
        command.ExecuteNonQuery();
    }
}
