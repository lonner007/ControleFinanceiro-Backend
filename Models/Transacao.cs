using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ControleFinanceiro_Backend.Models;
public class Transacao
{
    [Key] public int cdTransacao { get; set; }
    [Required] public int cdUsuario { get; set; }
    [ForeignKey("cdUsuario")] public Usuario? Usuario { get; set; }
    [Required][MaxLength(20)] public string tpTransacao { get; set; } = string.Empty;
    [Required] public int cdConta { get; set; }
    [ForeignKey("cdConta")] public Conta? Conta { get; set; }
    public int? cdContaDestino { get; set; }
    [ForeignKey("cdContaDestino")] public Conta? ContaDestino { get; set; }
    public int? cdCategoria { get; set; }
    [ForeignKey("cdCategoria")] public Categoria? Categoria { get; set; }
    [Required] public decimal vlTransacao { get; set; }
    [Required] public DateTime dtTransacao { get; set; }
    [MaxLength(500)] public string? dsTransacao { get; set; }
    public DateTime dtCriacao { get; set; } = DateTime.UtcNow;
}
