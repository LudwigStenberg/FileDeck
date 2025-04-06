using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FileDeck.api.DTOs.Auth;
using FileDeck.api.Models;
using FileDeck.api.Services;
using FileDeck.api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Npgsql.Replication;

namespace FileDeck.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ITokenService tokenService;
    private readonly IAuthService authService;
    private readonly UserManager<UserEntity> userManager;

    public AuthController(ITokenService tokenService, IAuthService authService, UserManager<UserEntity> userManager)
    {
        this.tokenService = tokenService;
        this.authService = authService;
        this.userManager = userManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUserAsync(RegisterRequestDto registerDto)
    {
        // Based on the attributes of RegisterRequestDto
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await authService.RegisterUserAsync(registerDto);


        if (result.Succeeded)
        {
            return CreatedAtAction(nameof(?))
        }

        return BadRequest(result);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetUserById(string id)
    {
        // Get the current user from the token
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (id != currentUserId)
        {
            return Forbid();
        }

        var user = await userManager.FindByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(new { user.Id, user.Email });
    }
}
