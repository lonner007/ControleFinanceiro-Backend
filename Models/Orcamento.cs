using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ControleFinanceiro_Backend.Models;
public class Orcamento
{
    [Key] public int cdOrcamento { get; set; }
    [Required] public int cdUsuario { get; set; }
    [ForeignKey("cdUsuario")] public Usuario? Usuario { get; set; }
    [Required] public int cdCategoria { get; set; }
    [ForeignKey("cdCategoria")] public Categoria? Categoria { get; set; }
    [Required] public decimal vlLimite { get; set; }
    [Required] public int nrMes { get; set; }
    [Required] public int nrAno { get; set; }
    public DateTime dtCriacao { get; set; } = DateTime.UtcNow;
}
