using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using FileDeck.api.DTOs.Auth;
using FileDeck.api.Repositories;
using FileDeck.api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

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
    /// <param name="request">The registration information including email and password.</param>
    /// <returns>A RegisterResponse object containing the result of the registration attempt
    // including success status and any error messages if registration failed.</returns>
    public async Task<RegisterResponse> RegisterUserAsync(RegisterRequest request)
    {
        logger.LogInformation("User registration attempt for email: {Email}", request.Email);


        if (request.Password != request.ConfirmPassword)
        {
            logger.LogWarning("Registration failed: passwords do not match for {Email}", request.Email);
            throw new ValidationException("Passwords do not match");
        }

        var existingUser = await authRepository.FindUserByEmailAsync(request.Email);
        if (existingUser != null)
        {
            logger.LogWarning("Registration failed: email {Email} is already in use", request.Email);
            throw new UserAlreadyExistsException(request.Email);
        }

        var newUser = UserMapper.ToEntity(request);

        logger.LogDebug("Attempting to create new user with email: {Email}", request.Email);
        IdentityResult result = await authRepository.CreateUserAsync(newUser, request.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            logger.LogWarning("User registration failed for {Email}. Errors: {Errors}", request.Email, string.Join(", ", errors));
            throw new ValidationException(string.Join(", ", errors));
        }

        logger.LogDebug("User registered successfully: {UserId}, {Email}", newUser.Id, newUser.Email);
        return UserMapper.ToRegisterResponse(newUser);
    }

    /// <summary>
    /// Signs in an existing user based on the provided login information.
    /// </summary>
    /// <param name="request">The login information including email and password.</param>
    /// <returns>A LoginResponse object containing the result of the login attempt including success status, potential errors and a newly generated token if the attempt is successful.</returns>
    public async Task<LoginResponse> LoginUserAsync(LoginRequest request)
    {
        logger.LogInformation("User login attempt for email: {Email}", request.Email);

        var user = await authRepository.FindUserByEmailAsync(request.Email);

        if (user == null)
        {
            logger.LogWarning("Login attempt failed for email: {Email}. The email or password is incorrect", request.Email);
            return UserMapper.ToFailedLoginResponse(new List<string> { "The email or password is incorrect." });
        }

        logger.LogDebug("Attempting to validate password for email: {Email}", request.Email);

        var result = await authRepository.CheckPasswordSignInAsync(user, request.Password);

        if (result.Succeeded)
        {
            logger.LogInformation("User password successfully checked for email: {Email}. Generating token...", request.Email);

            var tokenResult = tokenService.GenerateToken(user);

            logger.LogInformation("User successfully logged in: {UserId}, {Email}", user.Id, user.Email);
            return UserMapper.ToSuccessfulLoginResponse(user, tokenResult);
        }

        logger.LogWarning("User login failed for email: {Email}. Incorrect email or password.", request.Email);
        return UserMapper.ToFailedLoginResponse(new List<string> { "The email or password is incorrect." });
    }
}