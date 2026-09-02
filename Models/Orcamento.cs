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
    [Required][Range(typeof(decimal), "0.01", "1000000000")] public decimal vlLimite { get; set; }
    [Required][Range(1, 12)] public int nrMes { get; set; }
    [Required][Range(2000, 2100)] public int nrAno { get; set; }
    public DateTime dtCriacao { get; set; } = DateTime.UtcNow;
}
