using E_Commerce.Shared.DTOs.BasketDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Services_Abstraction
{
    public interface IBaskerService
    {
        Task<BasketDTO> GetBasketAsync(string id);
        Task<BasketDTO> CreareOrUpdateBasketAsync(BasketDTO basket);
        Task<bool> DeleteBasketAsync(string Id);

    }
}
