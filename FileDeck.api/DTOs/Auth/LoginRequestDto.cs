
using System.ComponentModel.DataAnnotations;

namespace FileDeck.api.DTOs.Auth;

public class LoginRequestDto
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 8)]
    public required string Password { get; set; }
}