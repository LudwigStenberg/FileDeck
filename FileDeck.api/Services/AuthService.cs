using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileDeck.api.DTOs.Auth;
using FileDeck.api.Models;
using FileDeck.api.Repositories;
using FileDeck.api.Services.Interfaces;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace FileDeck.api.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository authRepository;
    private readonly ITokenService tokenService;
    public AuthService(IAuthRepository authRepository, ITokenService tokenService)
    {
        this.authRepository = authRepository;
        this.tokenService = tokenService;
    }

    /// <summary>
    /// Registers new user in the system based on the provided registration information.
    /// </summary>
    /// <param name="registerDto">The registration information including email and password.</param>
    /// <returns>A RegisterResponseDto object containing the result of the registration attempt
    // including success status and any error messages if registration failed.</returns>
    public async Task<RegisterResponseDto> RegisterUserAsync(RegisterRequestDto registerDto)
    {
        // Check if passwords match:
        if (registerDto.Password != registerDto.ConfirmPassword)
        {
            return new RegisterResponseDto
            {
                Succeeded = false,
                Errors = new List<string> { "Passwords do not match" }
            };
        }

        // Check if the email is already in use:
        var existingUser = await authRepository.FindUserByEmailAsync(registerDto.Email);
        if (existingUser != null)
        {
            return new RegisterResponseDto
            {
                Succeeded = false,
                Errors = new List<string> { "Email is already in use" }
            };
        }

        // Create the user:
        var newUser = new UserEntity
        {
            UserName = registerDto.Email,
            Email = registerDto.Email
        };

        IdentityResult result = await authRepository.CreateUserAsync(newUser, registerDto.Password);

        if (result.Succeeded)
        {
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
        var user = await authRepository.FindUserByEmailAsync(loginDto.Email);

        // Check Email:
        if (user == null)
        {
            return new LoginResponseDto
            {
                Succeeded = false,
                Errors = new List<string> { "The email or password is incorrect" }
            };
        }

        // Check Password:
        var result = await authRepository.CheckPasswordSignInAsync(user, loginDto.Password);

        if (result.Succeeded)
        {
            var token = tokenService.GenerateToken(user);

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

        return new LoginResponseDto
        {
            Succeeded = false,
            Errors = new List<string> { "The email or password is incorrect" }
        };
    }
}