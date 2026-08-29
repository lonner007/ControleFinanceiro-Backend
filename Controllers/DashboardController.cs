using ControleFinanceiro_Backend.Extensions;
using ControleFinanceiro_Backend.Repositorios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ControleFinanceiro_Backend.Controllers;
[Authorize][ApiController][Route("Api/Dashboard")]
public class DashboardController : ControllerBase
{
    private readonly OrcamentoRepositorio _repo;
    public DashboardController(OrcamentoRepositorio repo) { _repo = repo; }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int? mes, [FromQuery] int? ano)
    {
        var resumo = await _repo.ObterDashboard(User.GetUserId(), mes ?? DateTime.UtcNow.Month, ano ?? DateTime.UtcNow.Year);
        return Ok(resumo);
    }
}
