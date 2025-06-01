
using FileDeck.api.Models;

namespace FileDeck.api.Services;

public interface ITokenService
{
    TokenResult GenerateToken(UserEntity user);
}

