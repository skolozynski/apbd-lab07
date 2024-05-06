using Lab07.Models.DTOs;
using Lab07.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Lab07.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class WarehousesController : ControllerBase
{
    private readonly IWarehouseRepository _warehouseRepository;
    
    public WarehousesController(IWarehouseRepository warehouseRepository)
    {
        _warehouseRepository = warehouseRepository;
    }

    [HttpPost]
    public async Task<IActionResult> AddProductToWarehouse(AddProductToWarehouseDTO addProductToWarehouseDto)
    {
        if (!addProductToWarehouseDto.IsAmountGraterThanZero())
        {
            return BadRequest("Amount must be grater than 0");
        }

        if (! await _warehouseRepository.DoesWarehouseExist(addProductToWarehouseDto.IdWarehouse))
        {
            return NotFound($"Warehouse with id {addProductToWarehouseDto.IdWarehouse} does not exist");
        }

        if (!await _warehouseRepository.DoesProductExist(addProductToWarehouseDto.IdProduct))
        {
            return NotFound($"Product with id {addProductToWarehouseDto.IdProduct} does not exist");
        }

        if (!await _warehouseRepository.DoesOrderExist(
                addProductToWarehouseDto.IdProduct,
                addProductToWarehouseDto.Amount,
                addProductToWarehouseDto.CreatedAt))
        {
            return NotFound($"Order for product with id {addProductToWarehouseDto.IdProduct} and amount of  " +
                            $"{addProductToWarehouseDto.Amount} after {addProductToWarehouseDto.CreatedAt} does not exist!");
        }

        var orderId = await _warehouseRepository.GetIdOrder(
            addProductToWarehouseDto.IdProduct,
            addProductToWarehouseDto.Amount, 
            addProductToWarehouseDto.CreatedAt);
        var price = await _warehouseRepository.GetPrice(addProductToWarehouseDto.IdProduct);
        var pk = await _warehouseRepository.AddProduct(addProductToWarehouseDto, orderId, price);

        return Ok($"Product with id {pk} added");
    }

    [HttpPost]
    [Route("/api/Warehouses/procedure")]
    public async Task<IActionResult> AddProductToWarehouseWithProcedure(AddProductToWarehouseDTO addProductToWarehouseDto)
    {
        var pk = await _warehouseRepository.AddProductWithProcedure(addProductToWarehouseDto);
        
        return Ok($"Product with id {pk} added");
    }
}