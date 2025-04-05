
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FileDeck.api.Models;
using Microsoft.AspNetCore.Identity;

namespace FileDeck.api.Repositories;

public interface IAuthRepository
{
    Task<UserEntity?> FindUserByEmailAsync(string email);
    Task<IdentityResult> CreateUserAsync(UserEntity user, string password);
    Task<SignInResult> CheckPasswordSignInAsync(UserEntity user, string password);
}