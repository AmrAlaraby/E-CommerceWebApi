using E_Commerce.Domain.Entities.ProductModule;
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
        public ProductWithTypeAndPrandSpecification() : base(null) 
        {
            AddInclude(p=>p.ProductType);
            AddInclude(p => p.ProductBrand);
        }
    }
}
