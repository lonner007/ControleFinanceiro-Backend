using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ControleFinanceiro_Backend.Models;
public class Transacao
{
    [Key] public int cdTransacao { get; set; }
    [Required] public int cdUsuario { get; set; }
    [ForeignKey("cdUsuario")] public Usuario? Usuario { get; set; }
    [Required][StringLength(20, MinimumLength = 1)] public string tpTransacao { get; set; } = string.Empty;
    [Required] public int cdConta { get; set; }
    [ForeignKey("cdConta")] public Conta? Conta { get; set; }
    public int? cdContaDestino { get; set; }
    [ForeignKey("cdContaDestino")] public Conta? ContaDestino { get; set; }
    public int? cdCategoria { get; set; }
    [ForeignKey("cdCategoria")] public Categoria? Categoria { get; set; }
    [Required][Range(typeof(decimal), "0.01", "1000000000")] public decimal vlTransacao { get; set; }
    [Required] public DateTime dtTransacao { get; set; }
    [MaxLength(500)] public string? dsTransacao { get; set; }
    // Parcelamento
    public bool parcelado { get; set; } = false;
    [Range(2, 120)] public int? nrParcelas { get; set; }
    [Range(1, 120)] public int? nrParcelaAtual { get; set; }
    public int? cdTransacaoPai { get; set; }
    public DateTime dtCriacao { get; set; } = DateTime.UtcNow;
}
