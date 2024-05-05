using System.ComponentModel.DataAnnotations;

namespace Lab07.Models.DTOs;

public class AddProductToWarehouseDTO
{
    [Required]
    public int IdProduct { get; set; }
    [Required]
    public int IdWarehouse { get; set; }
    [Required]
    public int Amount { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; }

    public bool IsAmountGraterThanZero()
    {
        return Amount > 0;
    }
}