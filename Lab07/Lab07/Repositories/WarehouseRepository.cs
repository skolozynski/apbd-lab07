using System.Data;
using Lab07.Models.DTOs;
using Microsoft.Data.SqlClient;

namespace Lab07.Repositories;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly IConfiguration _configuration;

    public WarehouseRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }


    public async Task<bool> DoesProductExist(int id)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        var query = "SELECT 1 FROM Product WHERE IdProduct = @id";
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@id", id);

        await connection.OpenAsync();
        var res = await command.ExecuteScalarAsync();
        return res != null;
    }

    public async Task<bool> DoesWarehouseExist(int id)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        var query = "SELECT 1 FROM Warehouse WHERE IdWarehouse = @id";
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@id", id);

        await connection.OpenAsync();
        var res = await command.ExecuteScalarAsync();
        return res != null;
    }
    
    public async Task<bool> DoesOrderExist(int id, int amount, DateTime createdAt)
    {
        await using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using var command = new SqlCommand();

        command.Connection = connection;
        command.CommandText =
            "SELECT 1 FROM [Order] WHERE IdProduct = @Id AND Amount = @Amount AND CreatedAt < @CreatedAt " +
            "AND FulfilledAt IS NULL";
        command.Parameters.AddWithValue("@Id", id);
        command.Parameters.AddWithValue("@Amount", amount);
        command.Parameters.AddWithValue("@CreatedAt", createdAt);

        await connection.OpenAsync();

        var result = await command.ExecuteScalarAsync();

        return result != null;
    }

    public async Task<int> GetIdOrder(int id, int amount, DateTime createdAt)
    {
        await using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using var command = new SqlCommand();
        
        command.Connection = connection;
        command.CommandText = "SELECT IdOrder FROM [Order] WHERE IdProduct = @Id AND Amount = @Amount AND CreatedAt < @CreatedAt AND FulfilledAt IS NULL";
        command.Parameters.AddWithValue("@Id", id);
        command.Parameters.AddWithValue("@Amount", amount);
        command.Parameters.AddWithValue("@CreatedAt", createdAt);
        
        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();

        return (int) (result ?? throw new InvalidOperationException());
    }

    public async Task<decimal> GetPrice(int id)
    {
        await using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using var command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = "SELECT Price FROM [Product] WHERE IdProduct = @Id";
        command.Parameters.AddWithValue("@Id", id);

        await connection.OpenAsync();

        var result = await command.ExecuteScalarAsync();

        return (decimal) (result ?? throw new InvalidOperationException());
    }

    public async Task<int> AddProduct(AddProductToWarehouseDTO addProductToWarehouseDto, int idOrder, decimal price)
    {
        await using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using var command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = "UPDATE [Order] SET FulfilledAt = @FulfilledAt WHERE IdOrder = @IdOrder";
        command.Parameters.AddWithValue("@FulfilledAt", DateTime.Now);
        command.Parameters.AddWithValue("@IdOrder", idOrder);

        await connection.OpenAsync();

        var transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction as SqlTransaction;

        object? id;

        try
        {
            await command.ExecuteScalarAsync();

            command.Parameters.Clear();
            command.CommandText = "INSERT INTO [Product_Warehouse] VALUES (@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, @CreatedAt); SELECT @@IDENTITY AS ID;";
            command.Parameters.AddWithValue("@IdWarehouse", addProductToWarehouseDto.IdWarehouse);
            command.Parameters.AddWithValue("@IdProduct", addProductToWarehouseDto.IdProduct);
            command.Parameters.AddWithValue("@IdOrder", idOrder);
            command.Parameters.AddWithValue("@Amount", addProductToWarehouseDto.Amount);
            command.Parameters.AddWithValue("@Price", price * addProductToWarehouseDto.Amount);
            command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

            id = await command.ExecuteScalarAsync();

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }

        return Convert.ToInt32(id);
    }

    public async Task<int> AddProductWithProcedure(AddProductToWarehouseDTO addProductToWarehouseDto)
    {
        await using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using var command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = "AddProductToWarehouse";
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@IdProduct", addProductToWarehouseDto.IdProduct);
        command.Parameters.AddWithValue("@IdWarehouse", addProductToWarehouseDto.IdWarehouse);
        command.Parameters.AddWithValue("@Amount", addProductToWarehouseDto.Amount);
        command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

        await connection.OpenAsync();

        var id = await command.ExecuteScalarAsync();

        return Convert.ToInt32(id);
    }
}