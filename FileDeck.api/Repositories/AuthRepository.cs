
using System.Threading.Tasks;
using FileDeck.api.Models;
using Microsoft.AspNetCore.Identity;

namespace FileDeck.api.Repositories;

public class AuthRepository : IAuthRepository
{

    private readonly UserManager<UserEntity> userManager;
    private readonly SignInManager<UserEntity> signInManager;
    public AuthRepository(UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
    }

    public async Task<UserEntity?> FindUserByEmailAsync(string email)
    {
        return await userManager.FindByEmailAsync(email);
    }

    public async Task<IdentityResult> CreateUserAsync(UserEntity user, string password)
    {
        return await userManager.CreateAsync(user, password);
    }

    public async Task<SignInResult> CheckPasswordSignInAsync(UserEntity user, string password)
    {
        return await signInManager.CheckPasswordSignInAsync(user, password, false);
    }
}