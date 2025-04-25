using System.Collections.Generic;

namespace FileDeck.api.DTOs.Auth;

public class RegisterResponse
{
    public bool Succeeded { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new List<string>();
}