using ControleFinanceiro_Backend.Extensions;
using ControleFinanceiro_Backend.Models;
using ControleFinanceiro_Backend.Repositorios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ControleFinanceiro_Backend.Controllers;
[Authorize][ApiController][Route("Api/Categorias")]
public class CategoriasController : ControllerBase
{
    private readonly CategoriaRepositorio _repo;
    public CategoriasController(CategoriaRepositorio repo) { _repo = repo; }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var lista = await _repo.ObterListaCategorias(User.GetUserId());
        return Ok(new RespostaHttp<List<Categoria>> { StatusCode = 200, Dados = lista, Mensagem = new List<Mensagem> { new() { titulo = "Sucesso", descricao = "Categorias carregadas", severity = TipoMensagem.Success } } });
    }

    [HttpPost("CriarCategoria")]
    public async Task<IActionResult> Criar([FromBody] Categoria categoria)
    { var res = await _repo.CriarCategoria(categoria, User.GetUserId()); return StatusCode(res.StatusCode, res); }

    [HttpPut("{cdCategoria}")]
    public async Task<IActionResult> Atualizar(int cdCategoria, [FromBody] Categoria categoria)
    { var res = await _repo.AtualizarCategoria(cdCategoria, categoria, User.GetUserId()); return StatusCode(res.StatusCode, res); }

    [HttpDelete("deletar/{cdCategoria}")]
    public async Task<IActionResult> Deletar(int cdCategoria)
    { var res = await _repo.DeletarCategoria(cdCategoria, User.GetUserId()); return StatusCode(res.StatusCode, res); }
}
