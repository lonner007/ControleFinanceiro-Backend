namespace ControleFinanceiro_Backend.DTOs;
public record RegistroDto(string nmUsuario, string dsEmail, string dsSenha);
public record LoginDto(string dsEmail, string dsSenha);
public record RefreshDto(string refreshToken);
public record AuthResponseDto(string accessToken, string refreshToken, int cdUsuario, string nmUsuario, string dsEmail);
