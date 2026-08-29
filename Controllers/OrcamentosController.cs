using ControleFinanceiro_Backend.Extensions;
using ControleFinanceiro_Backend.Models;
using ControleFinanceiro_Backend.Repositorios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ControleFinanceiro_Backend.Controllers;
[Authorize][ApiController][Route("Api/Orcamentos")]
public class OrcamentosController : ControllerBase
{
    private readonly OrcamentoRepositorio _repo;
    public OrcamentosController(OrcamentoRepositorio repo) { _repo = repo; }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int? mes, [FromQuery] int? ano)
    {
        var lista = await _repo.ObterOrcamentos(User.GetUserId(), mes ?? DateTime.UtcNow.Month, ano ?? DateTime.UtcNow.Year);
        return Ok(new RespostaHttp<List<object>> { StatusCode = 200, Dados = lista });
    }

    [HttpPost("Salvar")]
    public async Task<IActionResult> Salvar([FromBody] Orcamento orc)
    { var res = await _repo.SalvarOrcamento(orc, User.GetUserId()); return StatusCode(res.StatusCode, res); }

    [HttpDelete("deletar/{cdOrcamento}")]
    public async Task<IActionResult> Deletar(int cdOrcamento)
    { var res = await _repo.DeletarOrcamento(cdOrcamento, User.GetUserId()); return StatusCode(res.StatusCode, res); }
}
