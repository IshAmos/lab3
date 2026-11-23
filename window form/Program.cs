using window_form.Controllers;
using window_form.Data;
using window_form.Views;

namespace window_form;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        var database = new Database();
        database.Initialize();
        var repository = new MilkRepository(database);
        var collectionController = new CollectionController(repository);
        var salesController = new SalesController(repository);

        Application.Run(new MainForm(collectionController, salesController, database.DatabasePath));
    }
}