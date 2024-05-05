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

        return Ok("Created");
    }
}