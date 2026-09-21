
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.IdentityModule;
using E_Commerce.Persistence.Data.DataSeed;
using E_Commerce.Persistence.Data.DbContexts;
using E_Commerce.Persistence.IdentityData.DataSeed;
using E_Commerce.Persistence.IdentityData.DbContexts;
using E_Commerce.Persistence.Repositories;
using E_Commerce.Services;
using E_Commerce.Services.MappingProfiles;
using E_Commerce.Services_Abstraction;
using E_Commerce.web.CustomMiddleWares;
using E_Commerce.web.Extensions;
using E_Commerce.web.Factories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Text;

namespace E_Commerce.web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            #region Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token to authorize."
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
            builder.Services.AddDbContext<StoreDbContext>(
                options =>
                {
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DeafultConnection"));
                });
            builder.Services.AddKeyedScoped<IDataInitializer, DataInitializer>("Default");
            builder.Services.AddKeyedScoped<IDataInitializer, IdentityDataInitializer>("Identity");

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            //builder.Services.AddAutoMapper(x => x.AddProfile<ProductProfile>());
            //builder.Services.AddTransient<ProductPictureUrlResolver>();
            //builder.Services.AddAutoMapper(x => x.LicenseKey = "", typeof(ProductProfile).Assembly); //only works at development
            builder.Services.AddAutoMapper(typeof(ServicesAssemblyRefrence).Assembly);//downgrade to 14

            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                return ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("ResdisConnection")!);
            });
            builder.Services.AddScoped<IBasketRepository, BasketRepository>();
            builder.Services.AddScoped<IBasketService, BasketService>();
            builder.Services.AddScoped<ICacheRepository, CacheRepository>();
            builder.Services.AddScoped<ICacheService, CacheService>();
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = ApiResponseFactory.GenerateApiValidationResponse;
            });
            builder.Services.AddDbContext<StoreIdentityDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });
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

            //builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            //.AddEntityFrameworkStores<StoreIdentityDbContext>();
            builder.Services.AddIdentityCore<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<StoreIdentityDbContext>();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JWT:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:Audience"],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]!))
                };
            });
            #endregion

            var app = builder.Build();
            #region data seeding

            await app.MigrateDatabaseAsync();

            await app.MigrateIdentityDatabaseAsync();

            await app.SeedDatabaseAsync();

            await app.SeedIdentityDatabaseAsync();

            #endregion

            // Configure the HTTP request pipeline.

            #region Configure the HTTP request pipeline.


            app.UseMiddleware<ExceptionHandlerMiddleWare>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();


            app.UseStaticFiles();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run(); 
            #endregion
        }
    }
}
