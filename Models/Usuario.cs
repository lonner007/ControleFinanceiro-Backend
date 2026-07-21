using System.ComponentModel.DataAnnotations;

namespace ControleFinanceiro_Backend.Models;

public class Usuario
{
    [Key] public int cdUsuario { get; set; }
    [Required][MaxLength(100)] public string nmUsuario { get; set; } = string.Empty;
    [Required][MaxLength(200)] public string dsEmail { get; set; } = string.Empty;
    [Required] public string dsSenhaHash { get; set; } = string.Empty;
    public string? dsRefreshToken { get; set; }
    public DateTime? dtRefreshTokenExpira { get; set; }
    public bool ativo { get; set; } = true;
    public DateTime dtCriacao { get; set; } = DateTime.UtcNow;
}
