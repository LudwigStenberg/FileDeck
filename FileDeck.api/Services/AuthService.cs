using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileDeck.api.DTOs.Auth;
using FileDeck.api.Models;
using FileDeck.api.Repositories;
using FileDeck.api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FileDeck.api.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository authRepository;
    private readonly ITokenService tokenService;
    private readonly ILogger<AuthService> logger;
    public AuthService(
        IAuthRepository authRepository,
        ITokenService tokenService,
        ILogger<AuthService> logger)
    {
        this.authRepository = authRepository;
        this.tokenService = tokenService;
        this.logger = logger;
    }

    /// <summary>
    /// Registers new user in the system based on the provided registration information.
    /// </summary>
    /// <param name="registerDto">The registration information including email and password.</param>
    /// <returns>A RegisterResponseDto object containing the result of the registration attempt
    // including success status and any error messages if registration failed.</returns>
    public async Task<RegisterResponseDto> RegisterUserAsync(RegisterRequestDto registerDto)
    {
        logger.LogInformation("User registration attempt for email: {Email}", registerDto.Email);


        if (registerDto.Password != registerDto.ConfirmPassword)
        {
            logger.LogWarning("Registration failed: passwords do not match for {Email}", registerDto.Email);

            return new RegisterResponseDto
            {
                Succeeded = false,
                Errors = new List<string> { "Passwords do not match" }
            };
        }

        var existingUser = await authRepository.FindUserByEmailAsync(registerDto.Email);
        if (existingUser != null)
        {
            logger.LogWarning("Registration failed: email {Email} is already in use", registerDto.Email);

            return new RegisterResponseDto
            {
                Succeeded = false,
                Errors = new List<string> { "Email is already in use" }
            };
        }

        var newUser = new UserEntity
        {
            UserName = registerDto.Email,
            Email = registerDto.Email
        };

        logger.LogDebug("Attempting to create new user with email: {Email}", registerDto.Email);
        IdentityResult result = await authRepository.CreateUserAsync(newUser, registerDto.Password);

        if (result.Succeeded)
        {
            logger.LogDebug("User registered successfully: {UserId}, {Email}",
                             newUser.Id, newUser.Email);

            return new RegisterResponseDto
            {
                Succeeded = true,
                UserId = newUser.Id,
                Username = newUser.UserName ?? string.Empty,
                Email = newUser.Email ?? string.Empty,
            };
        }
        else
        {
            logger.LogWarning("User registration failed for {Email}. Errors: {Errors}",
                            registerDto.Email, string.Join(", ", result.Errors.Select(e => e.Description)));

            return new RegisterResponseDto
            {
                Succeeded = false,
                Errors = result.Errors.Select(error => error.Description).ToList()
            };
        }
    }

    /// <summary>
    /// Signs in an existing user based on the provided login information.
    /// </summary>
    /// <param name="loginDto">The login information including email and password.</param>
    /// <returns>A LoginResponseDto object containing the result of the login attempt including success status, potential errors and a newly generated token if the attempt is successful.</returns>
    public async Task<LoginResponseDto> LoginUserAsync(LoginRequestDto loginDto)
    {
        logger.LogInformation("User login attempt for email: {Email}", loginDto.Email);

        var user = await authRepository.FindUserByEmailAsync(loginDto.Email);

        if (user == null)
        {
            logger.LogWarning("Login attempt failed for email: {Email}. The email or password is incorrect", loginDto.Email);

            return new LoginResponseDto
            {
                Succeeded = false,
                Errors = new List<string> { "The email or password is incorrect" }
            };
        }

        logger.LogDebug("Attempting to validate password for email: {Email}", loginDto.Email);

        var result = await authRepository.CheckPasswordSignInAsync(user, loginDto.Password);

        if (result.Succeeded)
        {
            logger.LogInformation("User password successfully checked for email: {Email}. Generating token...", loginDto.Email);

            var token = tokenService.GenerateToken(user);

            logger.LogInformation("User successfully logged in: {UserId}, {Email}", user.Id, user.Email);

            return new LoginResponseDto
            {
                Succeeded = true,
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(240),
                UserId = user.Id,
                Username = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty
            };
        }

        logger.LogWarning("User login failed for email: {Email}. Incorrect email or password.", loginDto.Email);

        return new LoginResponseDto
        {
            Succeeded = false,
            Errors = new List<string> { "The email or password is incorrect" }
        };
    }
}