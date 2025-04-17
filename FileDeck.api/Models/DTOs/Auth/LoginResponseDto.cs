using System.Collections.Generic;
using System;

namespace FileDeck.api.DTOs.Auth;

public class LoginResponseDto
{
    public bool Succeeded { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new List<string>();
}
