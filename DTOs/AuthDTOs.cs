using System.ComponentModel.DataAnnotations;

namespace ControleFinanceiro_Backend.DTOs;

public class RegistroDto
{
    [Required, StringLength(120, MinimumLength = 2)]
    public string nmUsuario { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string dsEmail { get; set; } = string.Empty;

    [Required]
    [StringLength(128, MinimumLength = 10)]
    public string dsSenha { get; set; } = string.Empty;
}

public class LoginDto
{
    [Required, EmailAddress, StringLength(200)]
    public string dsEmail  { get; set; } = string.Empty;
    
    [Required, StringLength(128, MinimumLength = 10)]
    public string dsSenha  { get; set; } = string.Empty;
}

public record RefreshDto([property: Required, StringLength(512)] string refreshToken);
public record AuthResponseDto(string accessToken, string refreshToken, int cdUsuario, string nmUsuario, string dsEmail);
