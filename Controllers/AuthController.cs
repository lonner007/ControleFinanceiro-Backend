using System.Security.Claims;
using ControleFinanceiro_Backend.DTOs;
using ControleFinanceiro_Backend.Models;
using ControleFinanceiro_Backend.Repositorios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ControleFinanceiro_Backend.Controllers;

[ApiController]
[Route("Api/Auth")]
[EnableRateLimiting("auth")]
public class AuthController : ControllerBase
{
    private readonly AuthRepositorio _auth;
    private readonly ILogger<AuthController> _logger;
    public AuthController(AuthRepositorio auth, ILogger<AuthController> logger) { _auth = auth; _logger = logger; }

    private const string RefreshCookieName = "__Host-cf_refresh";

    private void DefinirRefreshCookie(string refreshToken, DateTime expiresUtc)
    {
        Response.Cookies.Append(RefreshCookieName, refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Strict,
            Path = "/",
            Expires = expiresUtc
        });
    }

    private void LimparRefreshCookie()
    {
        Response.Cookies.Delete(RefreshCookieName, new CookieOptions
        {
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Strict,
            Path = "/"
        });
    }

    [HttpPost("Registrar")]
    public async Task<IActionResult> Registrar([FromBody] RegistroDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.nmUsuario) || string.IsNullOrWhiteSpace(dto.dsEmail) || string.IsNullOrWhiteSpace(dto.dsSenha))
            return BadRequest(new { mensagem = new[] { new { titulo = "Erro", descricao = "Todos os campos são obrigatórios." } } });
        var res = await _auth.Registrar(dto);
        if (res.Dados != null)
        {
            DefinirRefreshCookie(res.Dados.refreshToken, DateTime.UtcNow.AddDays(7));
            _logger.LogInformation("Novo usuário registrado: {UserId}", res.Dados.cdUsuario);
        }
        return StatusCode(res.StatusCode, res);
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var res = await _auth.Login(dto);
        if (res.Dados != null)
        {
            DefinirRefreshCookie(res.Dados.refreshToken, DateTime.UtcNow.AddDays(7));
            _logger.LogInformation("Login bem-sucedido: {UserId}", res.Dados.cdUsuario);
        }
        else
            _logger.LogWarning("Falha de login para tentativa de e-mail normalizado.");
        return StatusCode(res.StatusCode, res);
    }

    [HttpPost("Refresh")]
    public async Task<IActionResult> Refresh([FromHeader(Name = "Authorization")] string? authHeader)
    {
        var accessToken = authHeader?.Replace("Bearer ", "") ?? "";
        var refreshToken = Request.Cookies[RefreshCookieName];
        if (string.IsNullOrWhiteSpace(refreshToken))
            return Unauthorized(new { mensagem = new[] { new { titulo = "Erro", descricao = "Sessão expirada." } } });
        var res = await _auth.RefreshToken(refreshToken, accessToken);
        if (res.Dados != null)
            DefinirRefreshCookie(res.Dados.refreshToken, DateTime.UtcNow.AddDays(7));
        return StatusCode(res.StatusCode, res);
    }

    [Authorize]
    [HttpPost("Logout")]
    public async Task<IActionResult> Logout()
    {
        var cdUsuario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _auth.Logout(cdUsuario);
        LimparRefreshCookie();
        _logger.LogInformation("Logout realizado: {UserId}", cdUsuario);
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
