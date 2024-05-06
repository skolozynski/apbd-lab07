using Lab07.Models.DTOs;

namespace Lab07.Repositories;

public interface IWarehouseRepository
{
    Task<bool> DoesProductExist(int id);
    Task<bool> DoesWarehouseExist(int id);
    Task<bool> DoesOrderExist(int id, int amount, DateTime createdAt);
    Task<int> GetIdOrder(int id, int amount, DateTime createdAt);
    Task<decimal> GetPrice(int id);
    Task<int> AddProduct(AddProductToWarehouseDTO addProductToWarehouseDto, int idOrder, decimal price);

    Task<int> AddProductWithProcedure(AddProductToWarehouseDTO addProductToWarehouseDto);
}