using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using FileDeck.api.Models;
using FileDeck.api.Repositories;
using Microsoft.AspNetCore.Identity;

namespace FileDeck.api.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository authRepository;
    private readonly ITokenService tokenService;
    private readonly UserManager<UserEntity> userManager;
    private readonly ILogger<AuthService> logger;
    public AuthService(
        IAuthRepository authRepository,
        ITokenService tokenService,
        UserManager<UserEntity> userManager,
        ILogger<AuthService> logger)
    {
        this.authRepository = authRepository;
        this.tokenService = tokenService;
        this.userManager = userManager;
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
    /// <returns>A LoginResponse DTO containing a token and its expiration, UserId and Email.</returns>
    /// <exception cref="InvalidCredentialException">Thrown when the provided Email or Password is incorrect.</exception>
    public async Task<LoginResponse> LoginUserAsync(LoginRequest request)
    {
        logger.LogInformation("User login attempt for email: {Email}", request.Email);

        var user = await authRepository.FindUserByEmailAsync(request.Email);

        if (user == null)
        {
            logger.LogWarning("Login attempt failed for email: {Email}. The email or password is incorrect", request.Email);
            throw new InvalidCredentialException("Incorrect email or password.");
        }

        logger.LogDebug("Attempting to validate password for email: {Email}", request.Email);

        var result = await authRepository.CheckPasswordSignInAsync(user, request.Password);

        if (!result.Succeeded)
        {
            logger.LogWarning("User login failed for email: {Email}. Incorrect email or password.", request.Email);
            throw new InvalidCredentialException("Incorrect email or password.");
        }

        logger.LogDebug("User password successfully checked for email: {Email}. Generating token...", request.Email);
        var tokenResult = tokenService.GenerateToken(user);

        logger.LogDebug("User successfully logged in: {UserId}, {Email}", user.Id, user.Email);
        return UserMapper.ToLoginResponse(user, tokenResult);
    }

    /// <summary>
    /// Retrieves information for the provided user.
    /// </summary>
    /// <param name="userId">The ID of the user requesting the retrieval and who should have access to it.</param>
    /// <returns>A UserResponse DTO containing information about the user.</returns>
    /// <exception cref="UserNotFoundException">Thrown when the user cannot be found.</exception>
    public async Task<UserResponse> GetUserById(string userId)
    {
        logger.LogInformation("Starting retrieval attempt for user with ID: '{userId}'", userId);
        var user = await userManager.FindByIdAsync(userId);

        if (user == null)
        {
            logger.LogWarning("User with ID: '{UserId}' could not be found.", userId);
            throw new UserNotFoundException(userId);
        }

        logger.LogDebug("User with ID: '{UserId}' successfully retrieved.", userId);
        return UserMapper.ToUserResponse(user);
    }
}