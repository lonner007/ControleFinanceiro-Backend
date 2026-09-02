using ControleFinanceiro_Backend.Extensions;
using ControleFinanceiro_Backend.Repositorios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
namespace ControleFinanceiro_Backend.Controllers;
[Authorize][ApiController][Route("Api/Dashboard")]
public class DashboardController : ControllerBase
{
    private readonly OrcamentoRepositorio _repo;
    public DashboardController(OrcamentoRepositorio repo) { _repo = repo; }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery, Range(1, 12)] int? mes, [FromQuery, Range(2000, 2100)] int? ano)
    {
        var resumo = await _repo.ObterDashboard(User.GetUserId(), mes ?? DateTime.UtcNow.Month, ano ?? DateTime.UtcNow.Year);
        return Ok(resumo);
    }
}
