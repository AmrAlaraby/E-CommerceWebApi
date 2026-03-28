using AutoMapper;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.ProductModule;
using E_Commerce.Services.Specifications;
using E_Commerce.Services_Abstraction;
using E_Commerce.Shared;
using E_Commerce.Shared.DTOs.ProductDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork ,IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
        }
        public async Task<IEnumerable<BrandDTO>> GetAllBrandsAsync()
        {
            var Brands = await _unitOfWork.GetRrepository<ProductBrand ,int>().GetAllAsync();
            return _mapper.Map<IEnumerable<BrandDTO>>(Brands);
        }

        public async Task<PaginatedResult<ProductDTO>> GetAllProductsAsync(ProductQueryParams queryParams)
        {
            var Repo = _unitOfWork.GetRrepository<Product, int>();
            var Spec = new ProductWithTypeAndPrandSpecification(queryParams);
            var Products = await Repo.GetAllAsync(Spec);
            var DataToReturn = _mapper.Map<IEnumerable<ProductDTO>>(Products);
            var CountOfReturnedData = DataToReturn.Count();
            var CountSpec = new ProductCountSpecifications(queryParams);
            var CountOfAllProducts = await Repo.CountAsync(CountSpec);
            return new PaginatedResult<ProductDTO>(queryParams.PageIndex, CountOfReturnedData, CountOfAllProducts, DataToReturn);
        }

        public async Task<IEnumerable<TypeDTO>> GetAllTypesAsync()
        {
            var Types = await _unitOfWork.GetRrepository<ProductType, int>().GetAllAsync();
            return _mapper.Map<IEnumerable<TypeDTO>>(Types);
        }

        public async Task<ProductDTO> GetProductAsync(int id)
        {
            var spec = new ProductWithTypeAndPrandSpecification(id);
            var Product = await _unitOfWork.GetRrepository<Product, int>().GetByIdAsync(spec);
            return _mapper.Map<ProductDTO>(Product);
        }
    }
}
