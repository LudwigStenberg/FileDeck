using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FileDeck.api.Models;
using FileDeck.api.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FileDeck.api.Services;

public class TokenService : ITokenService
{

    private readonly JwtSettings jwtSettings;
    private readonly ILogger<TokenService> logger;

    public TokenService(IOptions<JwtSettings> jwtSettings, ILogger<TokenService> logger)
    {
        this.jwtSettings = jwtSettings.Value;
        this.logger = logger;
    }

    /// <summary>
    /// Generates a new JWT token for authenticated user access.
    /// </summary>
    /// <param name="user">The user entity for which to generate the token.</param>
    /// <returns>A JWT token string containing the user identity claims, expiration date, and signature.</returns>
    public string GenerateToken(UserEntity user)
    {
        logger.LogInformation("Initiated token generation for user {UserId}", user.Id);

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings.SecretKey));

        // Create signing credentials using the security key and algorithm
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);


        // Create claims that will be included in the token
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
        };

        // Create the token descriptor with claims, expiration and signing credentials
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(jwtSettings.ExpirationInMinutes),
            SigningCredentials = creds,
            Issuer = jwtSettings.Issuer,
            Audience = jwtSettings.Audience
        };

        // Create token handler and generate the token from the Descriptor
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        logger.LogInformation("Token successfully generated for user {UserId}", user.Id);

        return tokenHandler.WriteToken(token);
    }
}