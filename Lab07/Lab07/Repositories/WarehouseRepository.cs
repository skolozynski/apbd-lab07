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
}