using FileDeck.api.Models;
using FileDeck.api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FileDeck.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ITokenService tokenService;

    public AuthController(ITokenService tokenService)
    {
        this.tokenService = tokenService;
    }

    // [HttpPost("register")]

}
