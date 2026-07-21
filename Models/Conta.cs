using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ControleFinanceiro_Backend.Models;
public class Conta
{
    [Key] public int cdConta { get; set; }
    [Required] public int cdUsuario { get; set; }
    [ForeignKey("cdUsuario")] public Usuario? Usuario { get; set; }
    public string nmConta { get; set; } = string.Empty;
    public int cdTipoConta { get; set; }
    public decimal vlSaldoInicial { get; set; }
    public decimal vlSaldoAtual { get; set; }
    public DateTime dtCriacao { get; set; } = DateTime.UtcNow;
}
