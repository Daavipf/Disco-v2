using System.Security.Claims;

namespace Disco.Extensions;

public static class ClaimsPrincipalExtensions
{
  public static Guid GetId(this ClaimsPrincipal user)
  {
    var id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
             ?? user.FindFirst("id")?.Value;

    if (string.IsNullOrEmpty(id))
    {
      throw new UnauthorizedAccessException("ID do usuário não encontrado no token.");
    }

    return Guid.Parse(id);
  }
}