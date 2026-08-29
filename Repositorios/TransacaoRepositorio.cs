using ControleFinanceiro_Backend.Data;
using ControleFinanceiro_Backend.Models;
using Microsoft.EntityFrameworkCore;
namespace ControleFinanceiro_Backend.Repositorios;
public class TransacaoRepositorio
{
    private readonly AppDbContext _context;
    public TransacaoRepositorio(AppDbContext context) { _context = context; }

    public async Task<List<object>> ObterListaTransacoes(int cdUsuario) =>
        await _context.Transacoes
            .Where(t => t.cdUsuario == cdUsuario)
            .Include(t => t.Conta).Include(t => t.ContaDestino).Include(t => t.Categoria)
            .OrderByDescending(t => t.dtTransacao)
            .Select(t => (object)new {
                t.cdTransacao, t.tpTransacao,
                t.cdConta, nmConta = t.Conta != null ? t.Conta.nmConta : null,
                t.cdContaDestino, nmContaDestino = t.ContaDestino != null ? t.ContaDestino.nmConta : null,
                t.cdCategoria, nmCategoria = t.Categoria != null ? t.Categoria.nmCategoria : null,
                t.vlTransacao, t.dtTransacao, t.dsTransacao,
                t.parcelado, t.nrParcelas, t.nrParcelaAtual, t.cdTransacaoPai,
                t.dtCriacao
            }).ToListAsync();

    public async Task<RespostaHttp<List<Transacao>>> CriarTransacao(Transacao transacao, int cdUsuario)
    {
        using var db = await _context.Database.BeginTransactionAsync();
        try
        {
            var conta = await _context.Contas.FirstOrDefaultAsync(c => c.cdConta == transacao.cdConta && c.cdUsuario == cdUsuario);
            if (conta == null) return ErroLista("Conta de origem não encontrada.");
            transacao.cdUsuario = cdUsuario;

            // Parcelamento
            if (transacao.parcelado && transacao.nrParcelas is > 1)
            {
                var vlParcela = Math.Round(transacao.vlTransacao / transacao.nrParcelas.Value, 2);
                var parcelas = new List<Transacao>();
                for (int i = 0; i < transacao.nrParcelas.Value; i++)
                {
                    var vl = i == transacao.nrParcelas.Value - 1
                        ? transacao.vlTransacao - vlParcela * (transacao.nrParcelas.Value - 1)
                        : vlParcela;
                    parcelas.Add(new Transacao
                    {
                        cdUsuario = cdUsuario, tpTransacao = transacao.tpTransacao,
                        cdConta = transacao.cdConta, cdCategoria = transacao.cdCategoria,
                        vlTransacao = vl, dtTransacao = transacao.dtTransacao.AddMonths(i),
                        dsTransacao = $"{transacao.dsTransacao} ({i + 1}/{transacao.nrParcelas.Value})",
                        parcelado = true, nrParcelas = transacao.nrParcelas,
                        nrParcelaAtual = i + 1, dtCriacao = DateTime.UtcNow
                    });
                    if (i == 0) { if (transacao.tpTransacao == "Despesa") conta.vlSaldoAtual -= vl; else conta.vlSaldoAtual += vl; }
                }
                _context.Transacoes.AddRange(parcelas);
                await _context.SaveChangesAsync();
                var pai = parcelas[0];
                foreach (var p in parcelas.Skip(1)) p.cdTransacaoPai = pai.cdTransacao;
                await _context.SaveChangesAsync();
                await db.CommitAsync();
                return new RespostaHttp<List<Transacao>> { StatusCode = 200, Dados = parcelas, Mensagem = new List<Mensagem> { new() { titulo = "Sucesso", descricao = $"{transacao.nrParcelas} parcelas criadas!", severity = TipoMensagem.Success } } };
            }

            // Transação simples
            if (transacao.tpTransacao == "Transferencia")
            {
                if (transacao.cdContaDestino == null) return ErroLista("Conta destino é obrigatória.");
                if (transacao.cdContaDestino == transacao.cdConta) return ErroLista("Conta origem e destino iguais.");
                var dest = await _context.Contas.FirstOrDefaultAsync(c => c.cdConta == transacao.cdContaDestino && c.cdUsuario == cdUsuario);
                if (dest == null) return ErroLista("Conta destino não encontrada.");
                conta.vlSaldoAtual -= transacao.vlTransacao;
                dest.vlSaldoAtual += transacao.vlTransacao;
            }
            else if (transacao.tpTransacao == "Receita") conta.vlSaldoAtual += transacao.vlTransacao;
            else if (transacao.tpTransacao == "Despesa") conta.vlSaldoAtual -= transacao.vlTransacao;

            transacao.dtCriacao = DateTime.UtcNow;
            _context.Transacoes.Add(transacao);
            await _context.SaveChangesAsync();
            await db.CommitAsync();
            return new RespostaHttp<List<Transacao>> { StatusCode = 200, Dados = new List<Transacao> { transacao }, Mensagem = new List<Mensagem> { new() { titulo = "Sucesso", descricao = "Transação registrada!", severity = TipoMensagem.Success } } };
        }
        catch (Exception ex) { await db.RollbackAsync(); return ErroLista(ex.Message); }
    }

    public async Task<RespostaHttp<Transacao>> AtualizarTransacao(int cdTransacao, Transacao nova, int cdUsuario)
    {
        using var db = await _context.Database.BeginTransactionAsync();
        try
        {
            var t = await _context.Transacoes.Include(x => x.Conta).Include(x => x.ContaDestino)
                .FirstOrDefaultAsync(x => x.cdTransacao == cdTransacao && x.cdUsuario == cdUsuario);
            if (t == null) return new RespostaHttp<Transacao> { StatusCode = 404, Mensagem = new List<Mensagem> { new() { titulo = "Não encontrado", descricao = "Transação não encontrada", severity = TipoMensagem.Error } } };
            await ReverterSaldo(t, cdUsuario);
            var novaConta = await _context.Contas.FirstOrDefaultAsync(c => c.cdConta == nova.cdConta && c.cdUsuario == cdUsuario);
            if (novaConta == null) return Erro("Conta não encontrada.");
            t.tpTransacao = nova.tpTransacao; t.cdConta = nova.cdConta; t.cdContaDestino = nova.cdContaDestino;
            t.cdCategoria = nova.cdCategoria; t.vlTransacao = nova.vlTransacao; t.dtTransacao = nova.dtTransacao; t.dsTransacao = nova.dsTransacao;
            if (nova.tpTransacao == "Transferencia")
            {
                var dest = await _context.Contas.FirstOrDefaultAsync(c => c.cdConta == nova.cdContaDestino && c.cdUsuario == cdUsuario);
                if (dest == null) return Erro("Conta destino não encontrada.");
                novaConta.vlSaldoAtual -= nova.vlTransacao; dest.vlSaldoAtual += nova.vlTransacao;
            }
            else if (nova.tpTransacao == "Receita") novaConta.vlSaldoAtual += nova.vlTransacao;
            else if (nova.tpTransacao == "Despesa") novaConta.vlSaldoAtual -= nova.vlTransacao;
            await _context.SaveChangesAsync(); await db.CommitAsync();
            return new RespostaHttp<Transacao> { StatusCode = 200, Dados = t, Mensagem = new List<Mensagem> { new() { titulo = "Sucesso", descricao = "Transação atualizada!", severity = TipoMensagem.Success } } };
        }
        catch (Exception ex) { await db.RollbackAsync(); return Erro(ex.Message); }
    }

    public async Task<RespostaHttp<Transacao>> DeletarTransacao(int cdTransacao, int cdUsuario)
    {
        using var db = await _context.Database.BeginTransactionAsync();
        try
        {
            var t = await _context.Transacoes.Include(x => x.Conta).Include(x => x.ContaDestino)
                .FirstOrDefaultAsync(x => x.cdTransacao == cdTransacao && x.cdUsuario == cdUsuario);
            if (t == null) return new RespostaHttp<Transacao> { StatusCode = 404, Mensagem = new List<Mensagem> { new() { titulo = "Não encontrado", descricao = "Transação não encontrada", severity = TipoMensagem.Error } } };
            await ReverterSaldo(t, cdUsuario);
            _context.Transacoes.Remove(t);
            await _context.SaveChangesAsync(); await db.CommitAsync();
            return new RespostaHttp<Transacao> { StatusCode = 200, Mensagem = new List<Mensagem> { new() { titulo = "Sucesso", descricao = "Transação deletada!", severity = TipoMensagem.Success } } };
        }
        catch (Exception ex) { await db.RollbackAsync(); return Erro(ex.Message); }
    }

    private async Task ReverterSaldo(Transacao t, int cdUsuario)
    {
        var conta = await _context.Contas.FirstOrDefaultAsync(c => c.cdConta == t.cdConta && c.cdUsuario == cdUsuario);
        if (conta == null) return;
        if (t.tpTransacao == "Receita") conta.vlSaldoAtual -= t.vlTransacao;
        else if (t.tpTransacao == "Despesa") conta.vlSaldoAtual += t.vlTransacao;
        else if (t.tpTransacao == "Transferencia")
        {
            conta.vlSaldoAtual += t.vlTransacao;
            if (t.cdContaDestino != null)
            {
                var dest = await _context.Contas.FirstOrDefaultAsync(c => c.cdConta == t.cdContaDestino && c.cdUsuario == cdUsuario);
                if (dest != null) dest.vlSaldoAtual -= t.vlTransacao;
            }
        }
    }

    private RespostaHttp<Transacao> Erro(string msg) => new() { StatusCode = 500, Mensagem = new List<Mensagem> { new() { titulo = "Erro", descricao = msg, severity = TipoMensagem.Error } } };
    private RespostaHttp<List<Transacao>> ErroLista(string msg) => new() { StatusCode = 500, Mensagem = new List<Mensagem> { new() { titulo = "Erro", descricao = msg, severity = TipoMensagem.Error } } };
}
