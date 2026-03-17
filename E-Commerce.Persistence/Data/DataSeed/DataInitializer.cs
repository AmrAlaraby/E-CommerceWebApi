using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.Entities.ProductModule;
using E_Commerce.Persistence.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace E_Commerce.Persistence.Data.DataSeed
{
    public class DataInitializer : IDataInitializer
    {
        private readonly StoreDbContext dbContext;

        public DataInitializer(StoreDbContext dbContext) {
            this.dbContext = dbContext;
        }
        public void Initialize()
        {
            try
            {
                var HasProduct = dbContext.Products.Any();
                var HasBrands = dbContext.ProductBrands.Any();
                var HasTypes = dbContext.ProductTypes.Any();

                if (HasProduct && HasBrands && HasTypes) return;

                if (!HasTypes)
                {
                    SeedDataFromJson<ProductType,int>("types.json",dbContext.ProductTypes);
                }
                if (!HasBrands)
                {
                    SeedDataFromJson<ProductBrand, int>("brands.json", dbContext.ProductBrands);
                }
                dbContext.SaveChanges();
                if (!HasProduct)
                {
                    SeedDataFromJson<Product, int>("products.json", dbContext.Products);
                }
                dbContext.SaveChanges();

            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Data seeding faild : {ex}");
            }
        }

        private void SeedDataFromJson<T , TKey>(string fileName,DbSet<T> dbSet) where T : BaseEntity<TKey> 
        {
            //D:\route back assignments\ecomerce\E-CommerceSolution\E-Commerce.Persistence\Data\DataSeed\JSONFiles\brands.json
            var FilePath = @"../E-Commerce.Persistence\Data\DataSeed\JSONFiles\"+ fileName;
            if(File.Exists(FilePath)) throw new FileNotFoundException($"File {fileName} dose not exist");
            try
            {
                using var datStream = File.OpenRead(FilePath);
                var data = JsonSerializer.Deserialize<List<T>>(datStream, new JsonSerializerOptions(){
                    PropertyNameCaseInsensitive = true
                });
                if (data != null) 
                {
                    dbSet.AddRange(data);
                }
            }
            catch(Exception ex) 
            {
                Console.WriteLine($"Error while Reading JSON File : {ex}");
            }
        }
    }
}
