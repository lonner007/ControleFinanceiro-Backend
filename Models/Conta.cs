using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ControleFinanceiro_Backend.Models;
public class Conta
{
    [Key] public int cdConta { get; set; }
    [Required] public int cdUsuario { get; set; }
    [ForeignKey("cdUsuario")] public Usuario? Usuario { get; set; }
    [Required][MaxLength(120)] public string nmConta { get; set; } = string.Empty;
    [Range(1, 20)] public int cdTipoConta { get; set; }
    [Range(typeof(decimal), "-1000000000", "1000000000")] public decimal vlSaldoInicial { get; set; }
    public decimal vlSaldoAtual { get; set; }
    public DateTime dtCriacao { get; set; } = DateTime.UtcNow;
}
