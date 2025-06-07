using Microsoft.EntityFrameworkCore;
using FileDeck.api.Data;
using FileDeck.api.Models;
using Microsoft.AspNetCore.Identity;
using FileDeck.api.Repositories.Interfaces;
using FileDeck.api.Services.Interfaces;
using FileDeck.api.Services;
using FileDeck.api.Repositories;
using FileDeck.api.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Diagnostics;
using System.ComponentModel.DataAnnotations;


namespace FileDeck.api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<FileDeckDbContext>(options => options.UseNpgsql(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            npgsqlOptions => npgsqlOptions.MigrationsAssembly("FileDeck.api")
        ));

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowReactApp",
                builder => builder
                    .WithOrigins("http://localhost:5173")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
        });

        builder.Services.Configure<JwtSettings>(
            builder.Configuration.GetSection("JwtSettings"));

        builder.Services.Configure<CleanupSettings>(
            builder.Configuration.GetSection("CleanupSettings"));


        // Add Identity services:
        builder.Services.AddIdentity<UserEntity, IdentityRole>(options =>
        {
            // Password settings
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

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"])),
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                ValidateAudience = true,
                ValidAudience = builder.Configuration["JwtSettings:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IFolderService, FolderService>();
        builder.Services.AddScoped<IFileService, FileService>();
        builder.Services.AddScoped<IAuthRepository, AuthRepository>();
        builder.Services.AddScoped<IFolderRepository, FolderRepository>();
        builder.Services.AddScoped<IFileRepository, FileRepository>();
        builder.Services.AddScoped<ICleanupService, CleanupService>();

        builder.Services.AddHostedService<CleanupBackgroundService>();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddOpenApi();

        var app = builder.Build();

        app.UseExceptionHandler(options =>
        {
            options.Run(async context =>
            {
                context.Response.ContentType = "application/json";
                var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;


                if (exception is ValidationException)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                }
                else if (exception is EmptyNameException or NameTooLongException or
                    InvalidCharactersException or FileEmptyException)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                }
                else if (exception is FileNotFoundException or FolderNotFoundException or UserNotFoundException)
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                }

                else if (exception is UserAlreadyExistsException)
                {
                    context.Response.StatusCode = StatusCodes.Status409Conflict;
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                }

                string errorMessage = exception switch
                {
                    ValidationException => exception.Message,

                    EmptyNameException => exception.Message,
                    NameTooLongException => exception.Message,
                    InvalidCharactersException => exception.Message,
                    FileEmptyException => exception.Message,

                    FileNotFoundException => exception.Message,
                    FolderNotFoundException => exception.Message,
                    UserNotFoundException => "User not found.",

                    UserAlreadyExistsException => exception.Message,
                    _ => "An unexpected error occurred."
                };

                await context.Response.WriteAsJsonAsync(new { error = errorMessage });
            });
        });


        // Configure the HTTP request pipeline (Add Middleware)
        if (app.Environment.IsDevelopment())
        {
            app.MapScalarApiReference();
        }

        app.MapOpenApi();
        app.UseHttpsRedirection();
        app.UseCors("AllowReactApp");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();


        app.Run();
    }
}