using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ControleFinanceiro_Backend.Models;
public class Categoria
{
    [Key] public int cdCategoria { get; set; }
    [Required] public int cdUsuario { get; set; }
    [ForeignKey("cdUsuario")] public Usuario? Usuario { get; set; }
    [Required][MaxLength(100)] public string nmCategoria { get; set; } = string.Empty;
    [MaxLength(500)] public string? dsCategoria { get; set; }
    [Required][StringLength(20, MinimumLength = 1)] public string tpCategoria { get; set; } = string.Empty;
    [MaxLength(50)] public string? icone { get; set; }
    [MaxLength(20)] public string? cor { get; set; }
    public bool ativo { get; set; } = true;
    public DateTime dtCriacao { get; set; } = DateTime.UtcNow;
}
