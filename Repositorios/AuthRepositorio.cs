using System.Security.Claims;
using ControleFinanceiro_Backend.Data;
using ControleFinanceiro_Backend.DTOs;
using ControleFinanceiro_Backend.Models;
using ControleFinanceiro_Backend.Servicos;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro_Backend.Repositorios;

public class AuthRepositorio
{
    private readonly AppDbContext _context;
    private readonly JwtServico _jwt;
    public AuthRepositorio(AppDbContext context, JwtServico jwt) { _context = context; _jwt = jwt; }

    public async Task<RespostaHttp<AuthResponseDto>> Registrar(RegistroDto dto)
    {
        try
        {
            if (await _context.Usuarios.AnyAsync(u => u.dsEmail == dto.dsEmail.ToLower().Trim()))
                return Erro("Este e-mail já está cadastrado.");

            if (dto.dsSenha.Length < 6)
                return Erro("A senha deve ter pelo menos 6 caracteres.");

            var usuario = new Usuario
            {
                nmUsuario = dto.nmUsuario.Trim(),
                dsEmail = dto.dsEmail.ToLower().Trim(),
                dsSenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.dsSenha),
                dtCriacao = DateTime.UtcNow,
                ativo = true,
            };

            var refreshToken = _jwt.GerarRefreshToken();
            usuario.dsRefreshToken = refreshToken;
            usuario.dtRefreshTokenExpira = DateTime.UtcNow.AddDays(7);

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return new RespostaHttp<AuthResponseDto>
            {
                StatusCode = 201,
                Dados = new AuthResponseDto(_jwt.GerarAccessToken(usuario), refreshToken, usuario.cdUsuario, usuario.nmUsuario, usuario.dsEmail),
                Mensagem = new List<Mensagem> { new() { titulo = "Sucesso", descricao = "Conta criada com sucesso!", severity = TipoMensagem.Success } }
            };
        }
        catch (Exception ex) { return Erro(ex.Message); }
    }

    public async Task<RespostaHttp<AuthResponseDto>> Login(LoginDto dto)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.dsEmail == dto.dsEmail.ToLower().Trim() && u.ativo);
        if (usuario == null || !BCrypt.Net.BCrypt.Verify(dto.dsSenha, usuario.dsSenhaHash))
            return Erro("E-mail ou senha incorretos.");
 
        var refreshToken = _jwt.GerarRefreshToken();
        usuario.dsRefreshToken = refreshToken;
        usuario.dtRefreshTokenExpira = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync();

        return new RespostaHttp<AuthResponseDto>
        {
            StatusCode = 200,
            Dados = new AuthResponseDto(_jwt.GerarAccessToken(usuario), refreshToken, usuario.cdUsuario, usuario.nmUsuario, usuario.dsEmail),
            Mensagem = new List<Mensagem> { new() { titulo = "Sucesso", descricao = "Login realizado!", severity = TipoMensagem.Success } }
        };
    }

    public async Task<RespostaHttp<AuthResponseDto>> RefreshToken(string refreshToken, string accessTokenAntigo)
    {
        var principal = _jwt.ObterPrincipalDoTokenExpirado(accessTokenAntigo);
        if (principal == null) return Erro("Token inválido.");

        var cdUsuario = int.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var usuario = await _context.Usuarios.FindAsync(cdUsuario);

        if (usuario == null || usuario.dsRefreshToken != refreshToken || usuario.dtRefreshTokenExpira < DateTime.UtcNow)
            return Erro("Refresh token inválido ou expirado.");

        var novoRefresh = _jwt.GerarRefreshToken();
        usuario.dsRefreshToken = novoRefresh;
        usuario.dtRefreshTokenExpira = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync();

        return new RespostaHttp<AuthResponseDto>
        {
            StatusCode = 200,
            Dados = new AuthResponseDto(_jwt.GerarAccessToken(usuario), novoRefresh, usuario.cdUsuario, usuario.nmUsuario, usuario.dsEmail),
        };
    }

    public async Task Logout(int cdUsuario)
    {
        var usuario = await _context.Usuarios.FindAsync(cdUsuario);
        if (usuario == null) return;
        usuario.dsRefreshToken = null;
        usuario.dtRefreshTokenExpira = null;
        await _context.SaveChangesAsync();
    }

    private RespostaHttp<AuthResponseDto> Erro(string msg) => new()
    {
        StatusCode = 400,
        Mensagem = new List<Mensagem> { new() { titulo = "Erro", descricao = msg, severity = TipoMensagem.Error } }
    };
}
