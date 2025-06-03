using System.Security.Claims;
using FileDeck.api.DTOs.Auth;
using FileDeck.api.Models;
using FileDeck.api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FileDeck.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService authService;
    private readonly UserManager<UserEntity> userManager;

    public AuthController(IAuthService authService, UserManager<UserEntity> userManager)
    {
        this.authService = authService;
        this.userManager = userManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await authService.RegisterUserAsync(request);

        return CreatedAtAction(nameof(GetUserById), new { id = result.UserId }, result);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetUserById(string id)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User ID not found in token" });
        }

        var user = await authService.GetUserById(id);

        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser(LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await authService.LoginUserAsync(request);

        return Ok(result);
    }
}
