using FileDeck.api.Models;
using FileDeck.api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FileDeck.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{

    private readonly UserManager<UserEntity> userManager;
    private readonly SignInManager<UserEntity> signInManager;
    private readonly ITokenService tokenService;

    public AuthController(
        UserManager<UserEntity> userManager,
        SignInManager<UserEntity> signInManager,
        ITokenService tokenService)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.tokenService = tokenService;
    }

}
