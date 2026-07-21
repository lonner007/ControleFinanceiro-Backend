using ControleFinanceiro_Backend.Data;
using ControleFinanceiro_Backend.Models;
using Microsoft.EntityFrameworkCore;
namespace ControleFinanceiro_Backend.Repositorios;
public class CategoriaRepositorio
{
    private readonly AppDbContext _context;
    public CategoriaRepositorio(AppDbContext context) { _context = context; }

    public async Task<List<Categoria>> ObterListaCategorias(int cdUsuario) =>
        await _context.Categorias.Where(c => c.cdUsuario == cdUsuario).OrderBy(c => c.nmCategoria).ToListAsync();

    public async Task<RespostaHttp<Categoria>> CriarCategoria(Categoria categoria, int cdUsuario)
    {
        try
        {
            categoria.cdUsuario = cdUsuario;
            categoria.dtCriacao = DateTime.UtcNow;
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();
            return Ok(categoria, "Categoria criada com sucesso");
        }
        catch (Exception ex) { return Erro(ex.Message); }
    }

    public async Task<RespostaHttp<Categoria>> AtualizarCategoria(int cdCategoria, Categoria nova, int cdUsuario)
    {
        try
        {
            var cat = await _context.Categorias.FirstOrDefaultAsync(c => c.cdCategoria == cdCategoria && c.cdUsuario == cdUsuario);
            if (cat == null) return NotFound("Categoria não encontrada");
            cat.nmCategoria = nova.nmCategoria; cat.dsCategoria = nova.dsCategoria;
            cat.tpCategoria = nova.tpCategoria; cat.icone = nova.icone; cat.cor = nova.cor; cat.ativo = nova.ativo;
            await _context.SaveChangesAsync();
            return Ok(cat, "Categoria atualizada com sucesso");
        }
        catch (Exception ex) { return Erro(ex.Message); }
    }

    public async Task<RespostaHttp<Categoria>> DeletarCategoria(int cdCategoria, int cdUsuario)
    {
        try
        {
            var cat = await _context.Categorias.FirstOrDefaultAsync(c => c.cdCategoria == cdCategoria && c.cdUsuario == cdUsuario);
            if (cat == null) return NotFound("Categoria não encontrada");
            _context.Categorias.Remove(cat);
            await _context.SaveChangesAsync();
            return Ok(null, "Categoria deletada com sucesso");
        }
        catch (Exception ex) { return Erro(ex.Message); }
    }

    private RespostaHttp<Categoria> Ok(Categoria? d, string msg) => new() { StatusCode = 200, Dados = d, Mensagem = new List<Mensagem> { new() { titulo = "Sucesso", descricao = msg, severity = TipoMensagem.Success } } };
    private RespostaHttp<Categoria> NotFound(string msg) => new() { StatusCode = 404, Mensagem = new List<Mensagem> { new() { titulo = "Não encontrado", descricao = msg, severity = TipoMensagem.Error } } };
    private RespostaHttp<Categoria> Erro(string msg) => new() { StatusCode = 500, Mensagem = new List<Mensagem> { new() { titulo = "Erro", descricao = msg, severity = TipoMensagem.Error } } };
}
