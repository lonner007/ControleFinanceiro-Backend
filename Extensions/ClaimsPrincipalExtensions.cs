using System.Security.Claims;
namespace ControleFinanceiro_Backend.Extensions;
public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user) =>
        int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("Usuário não autenticado."));
}
