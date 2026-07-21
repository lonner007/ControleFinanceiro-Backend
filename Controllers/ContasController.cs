using ControleFinanceiro_Backend.Extensions;
using ControleFinanceiro_Backend.Models;
using ControleFinanceiro_Backend.Repositorios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ControleFinanceiro_Backend.Controllers;
[Authorize][ApiController][Route("Api/Contas")]
public class ContasController : ControllerBase
{
    private readonly ContaRepositorio _repo;
    public ContasController(ContaRepositorio repo) { _repo = repo; }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var lista = await _repo.ObterListaContas(User.GetUserId());
        return Ok(new RespostaHttp<List<Conta>> { StatusCode = 200, Dados = lista, Mensagem = new List<Mensagem> { new() { titulo = "Sucesso", descricao = "Contas carregadas", severity = TipoMensagem.Success } } });
    }

    [HttpPost("CriarConta")]
    public async Task<IActionResult> Criar([FromBody] Conta conta)
    { var res = await _repo.CriarConta(conta, User.GetUserId()); return StatusCode(res.StatusCode, res); }

    [HttpPut("{cdConta}")]
    public async Task<IActionResult> Atualizar(int cdConta, [FromBody] Conta conta)
    { var res = await _repo.AtualizarConta(cdConta, conta, User.GetUserId()); return StatusCode(res.StatusCode, res); }

    [HttpDelete("deletar/{cdConta}")]
    public async Task<IActionResult> Deletar(int cdConta)
    { var res = await _repo.DeletarConta(cdConta, User.GetUserId()); return StatusCode(res.StatusCode, res); }
}
