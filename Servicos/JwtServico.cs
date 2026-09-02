using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ControleFinanceiro_Backend.Models;
using Microsoft.IdentityModel.Tokens;

namespace ControleFinanceiro_Backend.Servicos;

public class JwtServico
{
    private readonly IConfiguration _config;
    public JwtServico(IConfiguration config) { _config = config; }

    public string GerarAccessToken(Usuario usuario)
    {
        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Chave"]!));
        var creds = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.cdUsuario.ToString()),
            new Claim(ClaimTypes.Email, usuario.dsEmail),
            new Claim(ClaimTypes.Name, usuario.nmUsuario),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Emissor"],
            audience: _config["Jwt:Audiencia"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GerarRefreshToken()
    {
        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    public string HashRefreshToken(string refreshToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
        return Convert.ToBase64String(bytes);
    }

    public ClaimsPrincipal? ObterPrincipalDoTokenExpirado(string token)
    {
        var parametros = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Chave"]!)),
            ValidateIssuer = false, ValidateAudience = false, ValidateLifetime = false,
        };
        try
        {
            var principal = new JwtSecurityTokenHandler().ValidateToken(token, parametros, out var tokenValidado);
            if (tokenValidado is not JwtSecurityToken jwt || !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
                return null;
            return principal;
        }
        catch { return null; }
    }
}
