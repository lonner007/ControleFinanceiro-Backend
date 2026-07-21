using ControleFinanceiro_Backend.Extensions;
using ControleFinanceiro_Backend.Models;
using ControleFinanceiro_Backend.Repositorios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ControleFinanceiro_Backend.Controllers;
[Authorize][ApiController][Route("Api/Transacoes")]
public class TransacoesController : ControllerBase
{
    private readonly TransacaoRepositorio _repo;
    public TransacoesController(TransacaoRepositorio repo) { _repo = repo; }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var lista = await _repo.ObterListaTransacoes(User.GetUserId());
        return Ok(new RespostaHttp<List<object>> { StatusCode = 200, Dados = lista, Mensagem = new List<Mensagem> { new() { titulo = "Sucesso", descricao = "Transações carregadas", severity = TipoMensagem.Success } } });
    }

    [HttpPost("CriarTransacao")]
    public async Task<IActionResult> Criar([FromBody] Transacao transacao)
    { var res = await _repo.CriarTransacao(transacao, User.GetUserId()); return StatusCode(res.StatusCode, res); }

    [HttpPut("{cdTransacao}")]
    public async Task<IActionResult> Atualizar(int cdTransacao, [FromBody] Transacao transacao)
    { var res = await _repo.AtualizarTransacao(cdTransacao, transacao, User.GetUserId()); return StatusCode(res.StatusCode, res); }

    [HttpDelete("deletar/{cdTransacao}")]
    public async Task<IActionResult> Deletar(int cdTransacao)
    { var res = await _repo.DeletarTransacao(cdTransacao, User.GetUserId()); return StatusCode(res.StatusCode, res); }
}
