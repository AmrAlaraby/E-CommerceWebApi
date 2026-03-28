using E_Commerce.Domain.Entities.ProductModule;
using E_Commerce.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Services.Specifications
{
    internal class ProductWithTypeAndPrandSpecification :BaseSpecifications<Product,int>
    {
        //Product by id
        public ProductWithTypeAndPrandSpecification(int id) : base(p=>p.Id==id)
        {
            AddInclude(p => p.ProductType);
            AddInclude(p => p.ProductBrand);
        }
        //all Products
        public ProductWithTypeAndPrandSpecification(ProductQueryParams queryParams)
            : base(p=> (!queryParams.BrandId.HasValue || p.BrandId== queryParams.BrandId.Value) &&
                       (!queryParams.TypeId.HasValue ||p.TypeId== queryParams.TypeId.Value) &&
                       (string.IsNullOrEmpty(queryParams.Search) || p.Name.ToLower().Contains(queryParams.Search.ToLower())))
        {
            AddInclude(p=>p.ProductType);
            AddInclude(p => p.ProductBrand);
        }
    }
}
