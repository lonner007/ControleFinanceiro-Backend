using ControleFinanceiro_Backend.Data;
using ControleFinanceiro_Backend.Models;
using Microsoft.EntityFrameworkCore;
namespace ControleFinanceiro_Backend.Repositorios;
public class ContaRepositorio
{
    private readonly AppDbContext _context;
    public ContaRepositorio(AppDbContext context) { _context = context; }

    public async Task<List<Conta>> ObterListaContas(int cdUsuario) =>
        await _context.Contas.Where(c => c.cdUsuario == cdUsuario).ToListAsync();

    public async Task<RespostaHttp<Conta>> CriarConta(Conta conta, int cdUsuario)
    {
        try
        {
            conta.cdUsuario = cdUsuario; // Força o usuário do token — nunca do front
            conta.vlSaldoAtual = conta.vlSaldoInicial;
            conta.dtCriacao = DateTime.UtcNow;
            _context.Contas.Add(conta);
            await _context.SaveChangesAsync();
            return Ok(conta, "Conta criada com sucesso");
        }
        catch (Exception ex) { return Erro(ex.Message); }
    }

    public async Task<RespostaHttp<Conta>> AtualizarConta(int cdConta, Conta nova, int cdUsuario)
    {
        try
        {
            var conta = await _context.Contas.FirstOrDefaultAsync(c => c.cdConta == cdConta && c.cdUsuario == cdUsuario);
            if (conta == null) return NotFound("Conta não encontrada");
            var dif = nova.vlSaldoInicial - conta.vlSaldoInicial;
            conta.nmConta = nova.nmConta;
            conta.cdTipoConta = nova.cdTipoConta;
            conta.vlSaldoInicial = nova.vlSaldoInicial;
            conta.vlSaldoAtual += dif;
            await _context.SaveChangesAsync();
            return Ok(conta, "Conta atualizada com sucesso");
        }
        catch (Exception ex) { return Erro(ex.Message); }
    }

    public async Task<RespostaHttp<Conta>> DeletarConta(int cdConta, int cdUsuario)
    {
        try
        {
            var conta = await _context.Contas.FirstOrDefaultAsync(c => c.cdConta == cdConta && c.cdUsuario == cdUsuario);
            if (conta == null) return NotFound("Conta não encontrada");
            _context.Contas.Remove(conta);
            await _context.SaveChangesAsync();
            return Ok(null, "Conta deletada com sucesso");
        }
        catch (Exception ex) { return Erro(ex.Message); }
    }

    private RespostaHttp<Conta> Ok(Conta? d, string msg) => new() { StatusCode = 200, Dados = d, Mensagem = new List<Mensagem> { new() { titulo = "Sucesso", descricao = msg, severity = TipoMensagem.Success } } };
    private RespostaHttp<Conta> NotFound(string msg) => new() { StatusCode = 404, Mensagem = new List<Mensagem> { new() { titulo = "Não encontrado", descricao = msg, severity = TipoMensagem.Error } } };
    private RespostaHttp<Conta> Erro(string msg) => new() { StatusCode = 500, Mensagem = new List<Mensagem> { new() { titulo = "Erro", descricao = msg, severity = TipoMensagem.Error } } };
}
