using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace FileDeck.api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container:
        builder.Services.AddControllers();

        /* Enables API Endpoints Discovery - Helps Swagger work
        (Creates metadata: like a map of the API landscape) */
        builder.Services.AddEndpointsApiExplorer();

        // Adds the service that generates the Swagger document (JSON file that describes the API)
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
            app.UseSwaggerUI(config => config.SwaggerEndpoint("/swagger/v1/swagger.json", "File Storage Api v1")
            );
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        // Start the application
        app.Run();
    }
}