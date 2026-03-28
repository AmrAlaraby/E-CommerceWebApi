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
        public async Task InitializeAsync()
        {
            try
            {
                var HasProduct = await dbContext.Products.AnyAsync();
                var HasBrands = await dbContext.ProductBrands.AnyAsync();
                var HasTypes = await dbContext.ProductTypes.AnyAsync();

                if (HasProduct && HasBrands && HasTypes) return;

                if (!HasTypes)
                {
                    await SeedDataFromJsonAsync<ProductType,int>("types.json",dbContext.ProductTypes);
                }
                if (!HasBrands)
                {
                    await SeedDataFromJsonAsync<ProductBrand, int>("brands.json", dbContext.ProductBrands);
                }
                await dbContext.SaveChangesAsync();
                if (!HasProduct)
                {
                    await SeedDataFromJsonAsync<Product, int>("products.json", dbContext.Products);
                }
                await dbContext.SaveChangesAsync();

            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Data seeding faild : {ex}");
            }
        }

        private async Task SeedDataFromJsonAsync<T , TKey>(string fileName,DbSet<T> dbSet) where T : BaseEntity<TKey> 
        {
            //D:\route back assignments\ecomerce\E-CommerceSolution\E-Commerce.Persistence\Data\DataSeed\JSONFiles\brands.json
            var FilePath = @"../E-Commerce.Persistence\Data\DataSeed\JSONFiles\"+ fileName;
            if(!File.Exists(FilePath)) throw new FileNotFoundException($"File {fileName} dose not exist");
            try
            {
                using var datStream = File.OpenRead(FilePath);
                var data = await JsonSerializer.DeserializeAsync<List<T>>(datStream, new JsonSerializerOptions(){
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
