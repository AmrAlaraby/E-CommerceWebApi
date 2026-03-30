using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Shared.DTOs.BasketDTOs
{
    public record BasketItemDTO(
        int Id, 
        string Name , 
        string PictureUrl ,
        [Range(1,double.MaxValue)]
        decimal Price , 
        [Range(1,100)]
        int Quantity);
}