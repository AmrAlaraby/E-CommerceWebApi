
using E_Commerce.Domain.Contracts;
using E_Commerce.Persistence.Data.DataSeed;
using E_Commerce.Persistence.Data.DbContexts;
using E_Commerce.Persistence.Repositories;
using E_Commerce.Services;
using E_Commerce.Services.MappingProfiles;
using E_Commerce.Services_Abstraction;
using E_Commerce.web.Extensions;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<StoreDbContext>(
                options =>
                {
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DeafultConnection"));
                });
            builder.Services.AddScoped<IDataInitializer , DataInitializer>();
            builder.Services.AddScoped<IUnitOfWork , UnitOfWork>();

            //builder.Services.AddAutoMapper(x => x.AddProfile<ProductProfile>());
            //builder.Services.AddTransient<ProductPictureUrlResolver>();
            //builder.Services.AddAutoMapper(x => x.LicenseKey = "", typeof(ProductProfile).Assembly); //only works at development
            builder.Services.AddAutoMapper(typeof(ServicesAssemblyRefrence).Assembly);//downgrade to 14

            builder.Services.AddScoped<IProductService , ProductService>();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy =>
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    });
            });



            var app = builder.Build();

            app.UseCors("AllowAll");
            #region data seeding

            await app.MigrateDatabaseAsync();

            await app.SeedDatabaseAsync();

            #endregion

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();


            app.UseStaticFiles();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
