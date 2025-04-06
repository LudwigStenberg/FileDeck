using System.Linq;
using System.Threading.Tasks;
using FileDeck.api.DTOs.Auth;
using FileDeck.api.Models;
using FileDeck.api.Services;
using FileDeck.api.Services.Interfaces;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FileDeck.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ITokenService tokenService;
    private readonly IAuthService authService;

    public AuthController(ITokenService tokenService, IAuthService authService)
    {
        this.tokenService = tokenService;
        this.authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUserAsync(RegisterRequestDto registerDto)
    {
        if (registerDto == null)
        {
            return ...?
        }

        var registerResponseDto = await authService.RegisterUserAsync(registerDto);


        if (registerResponseDto.Succeeded)
        {
            return CreatedAtAction(nameof(?))
        }

        return registerResponseDto.Errors.ToList(e => e.Description.ToList());

    }



}
