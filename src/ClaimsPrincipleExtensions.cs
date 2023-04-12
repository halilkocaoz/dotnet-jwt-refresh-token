using System.Security.Claims;

namespace dotnet_jwt_refresh_token;

public static class ClaimsPrincipleExtensions
{
    public static string GetId(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}