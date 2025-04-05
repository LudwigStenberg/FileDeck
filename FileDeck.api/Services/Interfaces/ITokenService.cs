
using FileDeck.api.Models;

namespace FileDeck.api.Services;

public interface ITokenService
{
    string GenerateToken(UserEntity user);
}

