using Microsoft.AspNetCore.Routing.Constraints;

namespace ControleFinanceiro_Backend.Models
{
    public class RespostaHttp<T>
    {
        public int StatusCode { get; set; }
        public List<Mensagem> Mensagem { get; set; }
        public T? Dados { get; set; }
    }

    public class Mensagem
    {
        public string titulo { get; set; }
        public string descricao { get; set; }
        public TipoMensagem severity { get; set; }
    }

    public enum TipoMensagem
    {
        Success,
        Warn,
        Error
    }
}
