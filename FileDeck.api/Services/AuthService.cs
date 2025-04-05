using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileDeck.api.DTOs.Auth;
using FileDeck.api.Repositories;
using FileDeck.api.Services.Interfaces;

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

        throw new ArgumentException();
    }

    public async Task<LoginResponseDto> LoginUserAsync(LoginRequestDto loginDto)
    {
        throw new ArgumentException();
    }
}