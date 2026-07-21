using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ControleFinanceiro_Backend.Models;
public class Meta
{
    [Key] public int cdMeta { get; set; }
    [Required] public int cdUsuario { get; set; }
    [ForeignKey("cdUsuario")] public Usuario? Usuario { get; set; }
    [Required][MaxLength(150)] public string nmMeta { get; set; } = string.Empty;
    [MaxLength(500)] public string? dsMeta { get; set; }
    [Required] public decimal vlAlvo { get; set; }
    public decimal vlAtual { get; set; } = 0;
    [Required] public DateTime dtPrazo { get; set; }
    public bool ativo { get; set; } = true;
    public DateTime dtCriacao { get; set; } = DateTime.UtcNow;
}
