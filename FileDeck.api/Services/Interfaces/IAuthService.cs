
using System.Threading.Tasks;
using FileDeck.api.DTOs.Auth;

namespace FileDeck.api.Services.Interfaces;

public interface IAuthService
{
    Task<RegisterResponse> RegisterUserAsync(RegisterRequest request);
    Task<LoginResponse> LoginUserAsync(LoginRequest request);
}