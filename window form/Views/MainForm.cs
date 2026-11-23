using window_form.Controllers;
using window_form.Models;

namespace window_form.Views;

public sealed class MainForm : Form
{
    private readonly CollectionController _collectionController;
    private readonly SalesController _salesController;
    private readonly DataGridView _collectionGrid = CreateGrid();
    private readonly DataGridView _salesGrid = CreateGrid();
    private readonly TextBox _farmerName = new();
    private readonly DateTimePicker _collectionDate = CreateDatePicker();
    private readonly NumericUpDown _collectionQuantity = CreateNumberInput();
    private readonly NumericUpDown _collectionPrice = CreateNumberInput();
    private readonly TextBox _customerName = new();
    private readonly TextBox _collectionSearch = new();
    private readonly TextBox _salesSearch = new();
    private readonly DateTimePicker _saleDate = CreateDatePicker();
    private readonly NumericUpDown _saleQuantity = CreateNumberInput();
    private readonly NumericUpDown _salePrice = CreateNumberInput();
    private readonly Label _collectionSummary = new();
    private readonly Label _salesSummary = new();
    private readonly ToolStripStatusLabel _statusLabel = new();

    public MainForm(CollectionController collectionController, SalesController salesController, string databasePath)
    {
        _collectionController = collectionController;
        _salesController = salesController;
        Text = "Milk Management System";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(950, 600);
        Size = new Size(1100, 700);
        Font = new Font("Segoe UI", 10F);
        BackColor = Color.FromArgb(245, 248, 252);

        var tabs = new TabControl { Dock = DockStyle.Fill };
        tabs.Padding = new Point(14, 6);
        tabs.TabPages.Add(CreateCollectionPage());
        tabs.TabPages.Add(CreateSalesPage());
        Controls.Add(tabs);
        _statusLabel.Text = $"Database ready: {databasePath}";
        _statusLabel.Spring = true;
        var statusStrip = new StatusStrip { Dock = DockStyle.Bottom, BackColor = Color.FromArgb(226, 232, 240) };
        statusStrip.Items.Add(_statusLabel);
        Controls.Add(statusStrip);
        Load += (_, _) => RefreshAll();
    }

    private TabPage CreateCollectionPage()
    {
        var page = new TabPage("Milk Collection");
        var layout = CreatePageLayout();
        var form = CreateInputLayout();
        AddInput(form, "Farmer name", _farmerName, 0);
        AddInput(form, "Collection date", _collectionDate, 1);
        AddInput(form, "Quantity (litres)", _collectionQuantity, 2);
        AddInput(form, "Price per litre", _collectionPrice, 3);
        AddButtons(form, SaveCollection, UpdateCollection, DeleteCollection, ClearCollection, 4);
        layout.Controls.Add(form, 0, 0);
        layout.Controls.Add(CreateSearchPanel("Search farmer", _collectionSearch, RefreshCollections), 0, 1);
        layout.Controls.Add(_collectionGrid, 0, 2);
        ConfigureCollectionGrid();
        _collectionSummary.Text = "Total collected: 0.00 litres";
        layout.Controls.Add(_collectionSummary, 0, 3);
        page.Controls.Add(layout);
        _collectionGrid.SelectionChanged += (_, _) => LoadSelectedCollection();
        return page;
    }

    private TabPage CreateSalesPage()
    {
        var page = new TabPage("Milk Sales");
        var layout = CreatePageLayout();
        var form = CreateInputLayout();
        AddInput(form, "Customer name", _customerName, 0);
        AddInput(form, "Sale date", _saleDate, 1);
        AddInput(form, "Quantity (litres)", _saleQuantity, 2);
        AddInput(form, "Price per litre", _salePrice, 3);
        AddButtons(form, SaveSale, UpdateSale, DeleteSale, ClearSale, 4);
        layout.Controls.Add(form, 0, 0);
        layout.Controls.Add(CreateSearchPanel("Search customer", _salesSearch, RefreshSales), 0, 1);
        layout.Controls.Add(_salesGrid, 0, 2);
        ConfigureSalesGrid();
        _salesSummary.Text = "Total sold: 0.00 litres | Revenue: 0.00";
        layout.Controls.Add(_salesSummary, 0, 3);
        page.Controls.Add(layout);
        _salesGrid.SelectionChanged += (_, _) => LoadSelectedSale();
        return page;
    }

    private static TableLayoutPanel CreatePageLayout()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(12),
            RowCount = 4,
            ColumnCount = 1
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 115));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
        return layout;
    }

    private static TableLayoutPanel CreateInputLayout() => new()
    {
        Dock = DockStyle.Fill,
        ColumnCount = 5,
        RowCount = 2,
        AutoSize = true
    };

    private static void AddInput(TableLayoutPanel layout, string labelText, Control control, int column)
    {
        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(3) };
        panel.Controls.Add(new Label { Text = labelText, Dock = DockStyle.Top, AutoSize = true });
        panel.Controls[0].ForeColor = Color.FromArgb(55, 65, 81);
        control.Dock = DockStyle.Top;
        panel.Controls.Add(control);
        layout.Controls.Add(panel, column, 0);
    }

    private static void AddButtons(TableLayoutPanel layout, Action save, Action update, Action delete, Action clear, int column)
    {
        var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false, Padding = new Padding(3, 22, 0, 0) };
        panel.Controls.Add(CreateButton("Save", save));
        panel.Controls.Add(CreateButton("Update", update));
        panel.Controls.Add(CreateButton("Delete", delete));
        panel.Controls.Add(CreateButton("Clear", clear));
        layout.Controls.Add(panel, column, 0);
    }

    private static Control CreateSearchPanel(string labelText, TextBox searchBox, Action search)
    {
        var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false, Padding = new Padding(3) };
        panel.Controls.Add(new Label { Text = labelText, AutoSize = true, Padding = new Padding(0, 6, 6, 0) });
        searchBox.Width = 280;
        searchBox.PlaceholderText = "Type to search...";
        searchBox.TextChanged += (_, _) => search();
        panel.Controls.Add(searchBox);
        panel.Controls.Add(CreateButton("Refresh", search));
        return panel;
    }

    private static Button CreateButton(string text, Action action)
    {
        var button = new Button { Text = text, AutoSize = true, Padding = new Padding(8, 3, 8, 3), FlatStyle = FlatStyle.Flat, BackColor = text switch { "Save" => Color.FromArgb(22, 163, 74), "Update" => Color.FromArgb(37, 99, 235), "Delete" => Color.FromArgb(220, 38, 38), _ => Color.FromArgb(107, 114, 128) }, ForeColor = Color.White };
        button.FlatAppearance.BorderSize = 0;
        button.Click += (_, _) => action();
        return button;
    }

    private void ConfigureCollectionGrid()
    {
        _collectionGrid.Columns.Add("Id", "ID");
        _collectionGrid.Columns.Add("FarmerName", "Farmer");
        _collectionGrid.Columns.Add("CollectionDate", "Date");
        _collectionGrid.Columns.Add("QuantityLitres", "Quantity (L)");
        _collectionGrid.Columns.Add("PricePerLitre", "Price/L");
        _collectionGrid.Columns.Add("TotalAmount", "Total");
    }

    private void ConfigureSalesGrid()
    {
        _salesGrid.Columns.Add("Id", "ID");
        _salesGrid.Columns.Add("CustomerName", "Customer");
        _salesGrid.Columns.Add("SaleDate", "Date");
        _salesGrid.Columns.Add("QuantityLitres", "Quantity (L)");
        _salesGrid.Columns.Add("PricePerLitre", "Price/L");
        _salesGrid.Columns.Add("TotalAmount", "Total");
    }

    private void RefreshAll()
    {
        RefreshCollections();
        RefreshSales();
    }

    private void RefreshCollections()
    {
        _collectionGrid.Rows.Clear();
        var records = _collectionController.GetAll()
            .Where(item => string.IsNullOrWhiteSpace(_collectionSearch.Text) || item.FarmerName.Contains(_collectionSearch.Text.Trim(), StringComparison.OrdinalIgnoreCase))
            .ToList();
        foreach (var item in records)
        {
            _collectionGrid.Rows.Add(item.Id, item.FarmerName, item.CollectionDate.ToShortDateString(), item.QuantityLitres.ToString("N2"), item.PricePerLitre.ToString("C2"), item.TotalAmount.ToString("C2"));
        }
        _collectionSummary.Text = $"Total collected: {records.Sum(item => item.QuantityLitres):N2} litres | Value: {records.Sum(item => item.TotalAmount):C2}";
    }

    private void RefreshSales()
    {
        _salesGrid.Rows.Clear();
        var records = _salesController.GetAll()
            .Where(item => string.IsNullOrWhiteSpace(_salesSearch.Text) || item.CustomerName.Contains(_salesSearch.Text.Trim(), StringComparison.OrdinalIgnoreCase))
            .ToList();
        foreach (var item in records)
        {
            _salesGrid.Rows.Add(item.Id, item.CustomerName, item.SaleDate.ToShortDateString(), item.QuantityLitres.ToString("N2"), item.PricePerLitre.ToString("C2"), item.TotalAmount.ToString("C2"));
        }
        _salesSummary.Text = $"Total sold: {records.Sum(item => item.QuantityLitres):N2} litres | Revenue: {records.Sum(item => item.TotalAmount):C2}";
    }

    private void SaveCollection()
    {
        try
        {
            _collectionController.Save(new MilkCollection { FarmerName = _farmerName.Text.Trim(), CollectionDate = _collectionDate.Value.Date, QuantityLitres = _collectionQuantity.Value, PricePerLitre = _collectionPrice.Value });
            ClearCollection();
            RefreshCollections();
            SetStatus("Collection saved successfully to the database.");
        }
        catch (ArgumentException ex) { ShowValidationError(ex.Message); }
    }

    private void SaveSale()
    {
        try
        {
            _salesController.Save(new MilkSale { CustomerName = _customerName.Text.Trim(), SaleDate = _saleDate.Value.Date, QuantityLitres = _saleQuantity.Value, PricePerLitre = _salePrice.Value });
            ClearSale();
            RefreshSales();
            SetStatus("Sale saved successfully to the database.");
        }
        catch (ArgumentException ex) { ShowValidationError(ex.Message); }
    }

    private void UpdateCollection()
    {
        try
        {
            var id = GetSelectedId(_collectionGrid);
            if (id == 0) { ShowValidationError("Select a collection record to update."); return; }
            _collectionController.Save(new MilkCollection { Id = id, FarmerName = _farmerName.Text.Trim(), CollectionDate = _collectionDate.Value.Date, QuantityLitres = _collectionQuantity.Value, PricePerLitre = _collectionPrice.Value });
            ClearCollection();
            RefreshCollections();
            SetStatus("Collection updated successfully in the database.");
        }
        catch (ArgumentException ex) { ShowValidationError(ex.Message); }
    }

    private void UpdateSale()
    {
        try
        {
            var id = GetSelectedId(_salesGrid);
            if (id == 0) { ShowValidationError("Select a sale record to update."); return; }
            _salesController.Save(new MilkSale { Id = id, CustomerName = _customerName.Text.Trim(), SaleDate = _saleDate.Value.Date, QuantityLitres = _saleQuantity.Value, PricePerLitre = _salePrice.Value });
            ClearSale();
            RefreshSales();
            SetStatus("Sale updated successfully in the database.");
        }
        catch (ArgumentException ex) { ShowValidationError(ex.Message); }
    }

    private void DeleteCollection()
    {
        DeleteSelected(_collectionGrid, _collectionController.Delete, RefreshCollections, ClearCollection);
    }

    private void DeleteSale()
    {
        DeleteSelected(_salesGrid, _salesController.Delete, RefreshSales, ClearSale);
    }

    private static void DeleteSelected(DataGridView grid, Action<int> delete, Action refresh, Action clear)
    {
        var id = GetSelectedId(grid);
        if (id == 0) return;
        if (MessageBox.Show("Delete the selected record?", "Confirm deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            delete(id);
            clear();
            refresh();
            MessageBox.Show("The record was deleted from the database.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void LoadSelectedCollection()
    {
        if (_collectionGrid.CurrentRow is not { } row || row.Cells[0].Value is null) return;
        _farmerName.Text = row.Cells[1].Value?.ToString();
        _collectionDate.Value = ParseDate(row.Cells[2].Value?.ToString());
        _collectionQuantity.Value = ParseDecimal(row.Cells[3].Value?.ToString());
        _collectionPrice.Value = ParseCurrency(row.Cells[4].Value?.ToString());
    }

    private void LoadSelectedSale()
    {
        if (_salesGrid.CurrentRow is not { } row || row.Cells[0].Value is null) return;
        _customerName.Text = row.Cells[1].Value?.ToString();
        _saleDate.Value = ParseDate(row.Cells[2].Value?.ToString());
        _saleQuantity.Value = ParseDecimal(row.Cells[3].Value?.ToString());
        _salePrice.Value = ParseCurrency(row.Cells[4].Value?.ToString());
    }

    private void ClearCollection() { _collectionGrid.ClearSelection(); _farmerName.Clear(); _collectionDate.Value = DateTime.Today; _collectionQuantity.Value = 0; _collectionPrice.Value = 0; }
    private void ClearSale() { _salesGrid.ClearSelection(); _customerName.Clear(); _saleDate.Value = DateTime.Today; _saleQuantity.Value = 0; _salePrice.Value = 0; }

    private static int GetSelectedId(DataGridView grid) => grid.CurrentRow?.Cells[0].Value is { } value && int.TryParse(value.ToString(), out var id) ? id : 0;
    private static DateTime ParseDate(string? value) => DateTime.TryParse(value, out var date) ? date : DateTime.Today;
    private static decimal ParseDecimal(string? value) => decimal.TryParse(value, out var number) ? number : 0;
    private static decimal ParseCurrency(string? value) => decimal.TryParse(value, System.Globalization.NumberStyles.Currency, null, out var number) ? number : 0;
    private static void ShowValidationError(string message) => MessageBox.Show(message, "Validation error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    private void SetStatus(string message) => _statusLabel.Text = $"{message}  |  Database is connected";

    private static DataGridView CreateGrid() => new() { Dock = DockStyle.Fill, AllowUserToAddRows = false, ReadOnly = true, MultiSelect = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.FixedSingle, RowHeadersVisible = false, GridColor = Color.FromArgb(226, 232, 240), EnableHeadersVisualStyles = false, ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(30, 64, 175), ForeColor = Color.White, Font = new Font("Segoe UI", 10F, FontStyle.Bold), Alignment = DataGridViewContentAlignment.MiddleCenter }, AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(239, 246, 255) }, DefaultCellStyle = new DataGridViewCellStyle { SelectionBackColor = Color.FromArgb(191, 219, 254), SelectionForeColor = Color.Black } };
    private static DateTimePicker CreateDatePicker() => new() { Format = DateTimePickerFormat.Short };
    private static NumericUpDown CreateNumberInput() => new() { DecimalPlaces = 2, Maximum = 1000000, Increment = 0.5M, ThousandsSeparator = true };
}
