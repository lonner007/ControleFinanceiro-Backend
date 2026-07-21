using System.Security.Claims;
using ControleFinanceiro_Backend.DTOs;
using ControleFinanceiro_Backend.Models;
using ControleFinanceiro_Backend.Repositorios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControleFinanceiro_Backend.Controllers;

[ApiController]
[Route("Api/Auth")]
public class AuthController : ControllerBase
{
    private readonly AuthRepositorio _auth;
    public AuthController(AuthRepositorio auth) { _auth = auth; }

    [HttpPost("Registrar")]
    public async Task<IActionResult> Registrar([FromBody] RegistroDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.nmUsuario) || string.IsNullOrWhiteSpace(dto.dsEmail) || string.IsNullOrWhiteSpace(dto.dsSenha))
            return BadRequest(new { mensagem = new[] { new { titulo = "Erro", descricao = "Todos os campos são obrigatórios." } } });
        var res = await _auth.Registrar(dto);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var res = await _auth.Login(dto);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPost("Refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshDto dto, [FromHeader(Name = "Authorization")] string? authHeader)
    {
        var accessToken = authHeader?.Replace("Bearer ", "") ?? "";
        var res = await _auth.RefreshToken(dto.refreshToken, accessToken);
        return StatusCode(res.StatusCode, res);
    }

    [Authorize]
    [HttpPost("Logout")]
    public async Task<IActionResult> Logout()
    {
        var cdUsuario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _auth.Logout(cdUsuario);
        return Ok(new { mensagem = "Logout realizado." });
    }

    [Authorize]
    [HttpGet("Me")]
    public IActionResult Me() => Ok(new
    {
        cdUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier),
        nmUsuario = User.FindFirstValue(ClaimTypes.Name),
        dsEmail = User.FindFirstValue(ClaimTypes.Email),
    });
}
