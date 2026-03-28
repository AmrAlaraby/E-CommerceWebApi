using E_Commerce.Shared;
using E_Commerce.Shared.DTOs.ProductDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Services_Abstraction
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAllProductsAsync(ProductQueryParams queryParams);
        Task<ProductDTO> GetProductAsync(int id);
        Task<IEnumerable<TypeDTO>> GetAllTypesAsync();
        Task<IEnumerable<BrandDTO>> GetAllBrandsAsync();

    }
}
