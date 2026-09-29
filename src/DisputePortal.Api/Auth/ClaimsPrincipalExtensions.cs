using System.Security.Claims;

namespace DisputePortal.Api.Auth;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var sub = user.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? user.FindFirstValue("sub")
               ?? throw new UnauthorizedAccessException("The token does not contain a subject claim.");
        return Guid.Parse(sub);
    }
}