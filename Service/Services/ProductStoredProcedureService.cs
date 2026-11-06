using System.Data;
using System.Data.Common;
using System.Text.Json;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Service;

public class ProductStoredProcedureService(InventoryDbContext db) : IProductService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task<List<Product>> GetProducts()
    {
        var products = new List<Product>();
        await using (var command = await CreateCommand("dbo.csp_Inventory_Product_List"))
        await using (var reader = await command.ExecuteReaderAsync())
            while (await reader.ReadAsync()) products.Add(ReadProduct(reader));

        var lots = await GetLots();
        foreach (var lot in lots)
        {
            var product = products.FirstOrDefault(x => x.Id == lot.ProductId);
            if (product is not null) product.Lots.Add(lot);
        }
        return products;
    }

    public async Task<Product?> GetProduct(int id)
    {
        Product? product = null;
        await using (var command = await CreateCommand("dbo.csp_Inventory_Product_Get"))
        {
            AddParameter(command, "@Id", id);
            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync()) product = ReadProduct(reader);
        }
        if (product is not null) product.Lots = await GetLots(id);
        return product;
    }

    public async Task<Product> Add(Product product)
    {
        await using var command = await CreateCommand("dbo.csp_Inventory_Product_Create");
        AddProductParameters(command, product);
        int createdId;
        await using (var reader = await command.ExecuteReaderAsync())
        {
            if (!await reader.ReadAsync()) throw new InvalidOperationException("El procedimiento no devolvió el producto creado.");
            createdId = reader.GetInt32(0);
        }
        return await GetProduct(createdId) ?? throw new InvalidOperationException("No se encontró el producto creado.");
    }

    public async Task Update(Product product)
    {
        await using var command = await CreateCommand("dbo.csp_Inventory_Product_Update");
        AddParameter(command, "@Id", product.Id);
        AddProductParameters(command, product);
        await command.ExecuteNonQueryAsync();
    }

    public async Task Delete(int id)
    {
        await using var command = await CreateCommand("dbo.csp_Inventory_Product_Delete");
        AddParameter(command, "@Id", id);
        await command.ExecuteNonQueryAsync();
    }

    private async Task<List<ProductLot>> GetLots(int? productId = null)
    {
        var lots = new List<ProductLot>();
        await using var command = await CreateCommand("dbo.csp_Inventory_ProductLot_List");
        AddParameter(command, "@ProductId", productId);
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync()) lots.Add(new ProductLot { Id = reader.GetInt32(0), LotNumber = reader.GetString(1), Supplier = reader.GetString(2), Price = reader.GetDecimal(3), Stock = reader.GetInt32(4), ProductId = reader.GetInt32(5) });
        return lots;
    }

    private async Task<DbCommand> CreateCommand(string procedure)
    {
        var connection = db.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open) await connection.OpenAsync();
        var command = connection.CreateCommand();
        command.CommandText = procedure;
        command.CommandType = CommandType.StoredProcedure;
        return command;
    }

    private static Product ReadProduct(DbDataReader reader) => new()
    {
        Id = reader.GetInt32(0), Code = reader.GetString(1), Name = reader.GetString(2), Description = reader.IsDBNull(3) ? null : reader.GetString(3), IsActive = reader.GetBoolean(4),
    };

    private static void AddProductParameters(DbCommand command, Product product)
    {
        AddParameter(command, "@Code", product.Code); AddParameter(command, "@Name", product.Name); AddParameter(command, "@Description", product.Description); AddParameter(command, "@IsActive", product.IsActive);
        AddParameter(command, "@LotsJson", JsonSerializer.Serialize(product.Lots.Select(x => new { x.LotNumber, x.Supplier, x.Price, x.Stock }), JsonOptions));
    }

    private static void AddParameter(DbCommand command, string name, object? value)
    {
        var parameter = command.CreateParameter(); parameter.ParameterName = name; parameter.Value = value ?? DBNull.Value; command.Parameters.Add(parameter);
    }
}
