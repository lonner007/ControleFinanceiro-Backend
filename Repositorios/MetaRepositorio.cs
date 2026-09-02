using ControleFinanceiro_Backend.Data;
using ControleFinanceiro_Backend.Models;
using Microsoft.EntityFrameworkCore;
namespace ControleFinanceiro_Backend.Repositorios;
public class MetaRepositorio
{
    private readonly AppDbContext _context;
    public MetaRepositorio(AppDbContext context) { _context = context; }

    public async Task<List<Meta>> ObterListaMetas(int cdUsuario) =>
        await _context.Metas.AsNoTracking().Where(m => m.cdUsuario == cdUsuario).OrderBy(m => m.dtPrazo).Take(500).ToListAsync();

    public async Task<RespostaHttp<Meta>> CriarMeta(Meta meta, int cdUsuario)
    {
        try
        {
            meta.cdUsuario = cdUsuario;
            meta.dtCriacao = DateTime.UtcNow;
            _context.Metas.Add(meta);
            await _context.SaveChangesAsync();
            return Ok(meta, "Meta criada com sucesso");
        }
        catch { return Erro("Não foi possível concluir a operação."); }
    }

    public async Task<RespostaHttp<Meta>> AtualizarMeta(int cdMeta, Meta nova, int cdUsuario)
    {
        try
        {
            var meta = await _context.Metas.FirstOrDefaultAsync(m => m.cdMeta == cdMeta && m.cdUsuario == cdUsuario);
            if (meta == null) return NotFound("Meta não encontrada");
            meta.nmMeta = nova.nmMeta; meta.dsMeta = nova.dsMeta; meta.vlAlvo = nova.vlAlvo;
            meta.vlAtual = nova.vlAtual; meta.dtPrazo = nova.dtPrazo; meta.ativo = nova.ativo;
            await _context.SaveChangesAsync();
            return Ok(meta, "Meta atualizada com sucesso");
        }
        catch { return Erro("Não foi possível concluir a operação."); }
    }

    public async Task<RespostaHttp<Meta>> DeletarMeta(int cdMeta, int cdUsuario)
    {
        try
        {
            var meta = await _context.Metas.FirstOrDefaultAsync(m => m.cdMeta == cdMeta && m.cdUsuario == cdUsuario);
            if (meta == null) return NotFound("Meta não encontrada");
            _context.Metas.Remove(meta);
            await _context.SaveChangesAsync();
            return Ok(null, "Meta deletada com sucesso");
        }
        catch { return Erro("Não foi possível concluir a operação."); }
    }

    private RespostaHttp<Meta> Ok(Meta? d, string msg) => new() { StatusCode = 200, Dados = d, Mensagem = new List<Mensagem> { new() { titulo = "Sucesso", descricao = msg, severity = TipoMensagem.Success } } };
    private RespostaHttp<Meta> NotFound(string msg) => new() { StatusCode = 404, Mensagem = new List<Mensagem> { new() { titulo = "Não encontrado", descricao = msg, severity = TipoMensagem.Error } } };
    private RespostaHttp<Meta> Erro(string msg) => new() { StatusCode = 500, Mensagem = new List<Mensagem> { new() { titulo = "Erro", descricao = msg, severity = TipoMensagem.Error } } };
}
