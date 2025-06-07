
using FileDeck.api.Models;

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
    public static RegisterResponse ToRegisterResponse(UserEntity user)
    {
        return new RegisterResponse
        {
            UserId = user.Id,
            Email = user.Email ?? string.Empty
        };
    }

    public static LoginResponse ToLoginResponse(UserEntity user, TokenResult tokenResult)
    {
        return new LoginResponse
        {
            Token = tokenResult.Token,
            Expiration = tokenResult.Expiration,
            UserId = user.Id,
            Email = user.Email ?? string.Empty
        };
    }

    public static UserResponse ToUserResponse(UserEntity user)
    {
        return new UserResponse
        {
            UserId = user.Id,
            Email = user.Email
        };
    }
}