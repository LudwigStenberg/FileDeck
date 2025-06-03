
using System.Threading.Tasks;
using FileDeck.api.DTOs.Auth;
using FileDeck.api.Models;

namespace FileDeck.api.Services.Interfaces;

public interface IAuthService
{
    Task<RegisterResponse> RegisterUserAsync(RegisterRequest request);
    Task<LoginResponse> LoginUserAsync(LoginRequest request);
    Task<UserResponse> GetUserById(string userId);
}