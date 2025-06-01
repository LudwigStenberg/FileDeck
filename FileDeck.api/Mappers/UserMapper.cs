
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
            Email = user.Email
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
}