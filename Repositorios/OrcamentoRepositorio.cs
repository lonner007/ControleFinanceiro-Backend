using ControleFinanceiro_Backend.Data;
using ControleFinanceiro_Backend.Models;
using Microsoft.EntityFrameworkCore;
namespace ControleFinanceiro_Backend.Repositorios;
public class OrcamentoRepositorio
{
    private readonly AppDbContext _context;
    public OrcamentoRepositorio(AppDbContext context) { _context = context; }

    public async Task<List<object>> ObterOrcamentos(int cdUsuario, int mes, int ano)
    {
        var orcamentos = await _context.Orcamentos
            .Include(o => o.Categoria)
            .Where(o => o.cdUsuario == cdUsuario && o.nrMes == mes && o.nrAno == ano)
            .ToListAsync();
        var resultados = new List<object>();
        foreach (var orc in orcamentos)
        {
            var gasto = await _context.Transacoes
                .Where(t => t.cdUsuario == cdUsuario && t.cdCategoria == orc.cdCategoria
                    && t.tpTransacao == "Despesa"
                    && t.dtTransacao.Month == mes && t.dtTransacao.Year == ano)
                .SumAsync(t => (decimal?)t.vlTransacao) ?? 0;
            resultados.Add(new
            {
                orc.cdOrcamento, orc.cdCategoria,
                nmCategoria = orc.Categoria!.nmCategoria,
                corCategoria = orc.Categoria.cor,
                orc.vlLimite, vlGasto = gasto,
                vlDisponivel = orc.vlLimite - gasto,
                percentualUsado = orc.vlLimite > 0 ? Math.Round((gasto / orc.vlLimite) * 100, 1) : 0m,
                orc.nrMes, orc.nrAno
            });
        }
        return resultados;
    }

    public async Task<RespostaHttp<Orcamento>> SalvarOrcamento(Orcamento orc, int cdUsuario)
    {
        try
        {
            var catValida = await _context.Categorias.AnyAsync(c => c.cdCategoria == orc.cdCategoria && c.cdUsuario == cdUsuario);
            if (!catValida) return Erro("Categoria não encontrada.");
            var existente = await _context.Orcamentos.FirstOrDefaultAsync(
                o => o.cdUsuario == cdUsuario && o.cdCategoria == orc.cdCategoria && o.nrMes == orc.nrMes && o.nrAno == orc.nrAno);
            if (existente != null) { existente.vlLimite = orc.vlLimite; await _context.SaveChangesAsync(); return Ok("Orçamento atualizado!"); }
            orc.cdUsuario = cdUsuario;
            orc.dtCriacao = DateTime.UtcNow;
            _context.Orcamentos.Add(orc);
            await _context.SaveChangesAsync();
            return Ok("Orçamento criado!");
        }
        catch (Exception ex) { return Erro(ex.Message); }
    }

    public async Task<RespostaHttp<Orcamento>> DeletarOrcamento(int cdOrcamento, int cdUsuario)
    {
        try
        {
            var orc = await _context.Orcamentos.FirstOrDefaultAsync(o => o.cdOrcamento == cdOrcamento && o.cdUsuario == cdUsuario);
            if (orc == null) return new RespostaHttp<Orcamento> { StatusCode = 404, Mensagem = new List<Mensagem> { new() { titulo = "Não encontrado", descricao = "Orçamento não encontrado", severity = TipoMensagem.Error } } };
            _context.Orcamentos.Remove(orc);
            await _context.SaveChangesAsync();
            return Ok("Orçamento excluído!");
        }
        catch (Exception ex) { return Erro(ex.Message); }
    }

    public async Task<object> ObterDashboard(int cdUsuario, int mes, int ano)
    {
        var transacoes = await _context.Transacoes.Include(t => t.Categoria)
            .Where(t => t.cdUsuario == cdUsuario && t.dtTransacao.Month == mes && t.dtTransacao.Year == ano)
            .ToListAsync();
        var receitas = transacoes.Where(t => t.tpTransacao == "Receita").Sum(t => t.vlTransacao);
        var despesas = transacoes.Where(t => t.tpTransacao == "Despesa").Sum(t => t.vlTransacao);
        var porCategoria = transacoes
            .Where(t => t.tpTransacao == "Despesa" && t.Categoria != null)
            .GroupBy(t => new { t.cdCategoria, t.Categoria!.nmCategoria, t.Categoria.cor })
            .Select(g => new { g.Key.nmCategoria, g.Key.cor, total = g.Sum(t => t.vlTransacao) })
            .OrderByDescending(x => x.total).Take(5).ToList();
        var contas = await _context.Contas.Where(c => c.cdUsuario == cdUsuario).ToListAsync();
        var metas = await _context.Metas.Where(m => m.cdUsuario == cdUsuario && m.ativo).ToListAsync();
        return new
        {
            mes, ano,
            totalReceitas = receitas, totalDespesas = despesas,
            saldo = receitas - despesas,
            saldoTotal = contas.Sum(c => c.vlSaldoAtual),
            topCategorias = porCategoria,
            totalTransacoes = transacoes.Count,
            totalMetas = metas.Count,
            metasConcluidas = metas.Count(m => m.vlAtual >= m.vlAlvo)
        };
    }

    private RespostaHttp<Orcamento> Ok(string msg) => new() { StatusCode = 200, Mensagem = new List<Mensagem> { new() { titulo = "Sucesso", descricao = msg, severity = TipoMensagem.Success } } };
    private RespostaHttp<Orcamento> Erro(string msg) => new() { StatusCode = 500, Mensagem = new List<Mensagem> { new() { titulo = "Erro", descricao = msg, severity = TipoMensagem.Error } } };
}
