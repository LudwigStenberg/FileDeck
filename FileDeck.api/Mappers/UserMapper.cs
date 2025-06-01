
using FileDeck.api.DTOs.Auth;
using FileDeck.api.Models;
using Microsoft.Extensions.Configuration.UserSecrets;

public static class UserMapper
{

    public static UserEntity ToEntity(RegisterRequest request)
    {
        return new UserEntity
        {
            UserName = request.Email,
            Email = request.Email
        };
    }
    public static RegisterResponse ToSuccessfulRegisterResponse(UserEntity user)
    {
        return new RegisterResponse
        {
            Succeeded = true,
            UserId = user.Id,
            Email = user.Email ?? string.Empty
        };
    }

    public static RegisterResponse ToFailedRegisterResponse(List<string> errors)
    {
        return new RegisterResponse
        {
            Succeeded = false,
            Errors = errors
        };
    }

    public static LoginResponse ToSuccessfulLoginResponse(UserEntity user, TokenResult tokenResult)
    {
        return new LoginResponse
        {
            Succeeded = true,
            Token = tokenResult.Token,
            Expiration = tokenResult.Expiration,
            UserId = user.Id,
            Email = user.Email ?? string.Empty
        };
    }

    public static LoginResponse ToFailedLoginResponse(List<string> errors)
    {
        return new LoginResponse
        {
            Succeeded = false,
            Errors = errors
        };
    }
}