using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using FileDeck.api.Data;
using FileDeck.api.Models;
using Microsoft.AspNetCore.Identity;
using FileDeck.api.Repositories.Interfaces;
using FileDeck.api.Services.Interfaces;
using FileDeck.api.Services;
using FileDeck.api.Repositories;
using FileDeck.api.Settings;

namespace FileDeck.api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add Database context with PostgreSQL provider
        builder.Services.AddDbContext<FileDeckDbContext>(options => options.UseNpgsql(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            npgsqlOptions => npgsqlOptions.MigrationsAssembly("FileDeck.api")
        ));

        builder.Services.Configure<JwtSettings>(
            builder.Configuration.GetSection("JwtSettings"));


        // Add Identity services:
        builder.Services.AddIdentity<UserEntity, IdentityRole>(options =>
        {
            //Password settings
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;

            // User settings
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<FileDeckDbContext>()
        .AddDefaultTokenProviders();

        builder.Services.AddScoped<IFolderRepository, FolderRepository>();
        builder.Services.AddScoped<IFolderService, FolderService>();

        // Add services to the container:
        builder.Services.AddControllers();
        // Enables API Endpoints Discovery - Helps Swagger work
        // (Creates metadata: like a map of the API landscape)
        builder.Services.AddEndpointsApiExplorer();

        // Adds the Swagger documentation service (JSON file that describes the API)
        builder.Services.AddSwaggerGen(config =>
        {
            config.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "My API",
                Version = "v1",
                Description = "An API to perform file storage operations",
            });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline (Add Middleware)
        if (app.Environment.IsDevelopment())
        {
            //Enable Swagger UI in development:
            app.UseSwagger();
            app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "File Storage Api v1")
            );
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        // Start the application
        app.Run();
    }
}