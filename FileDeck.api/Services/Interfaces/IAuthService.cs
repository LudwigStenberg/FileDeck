
using System.Threading.Tasks;
using FileDeck.api.DTOs.Auth;

namespace FileDeck.api.Services.Interfaces;

public interface IAuthService
{
    Task<RegisterResponseDto> RegisterUserAsync(RegisterRequestDto registerDto);
    Task<LoginResponseDto> LoginUserAsync(LoginRequestDto loginDto);
}