using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using dotnet_jwt_refresh_token.Models;
using Microsoft.IdentityModel.Tokens;

namespace dotnet_jwt_refresh_token.Services;

public interface ITokenService
{
    /// <summary>
    /// Gets the principal claims from the exist token.
    /// </summary>
    /// <param name="bearerToken"></param>
    /// <returns></returns>
    ClaimsPrincipal GetClaimsPrincipal(string bearerToken);

    /// <summary>
    /// Creates a new token with refresh token value.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    TokenModel Create(string userId);
}


public class TokenService : ITokenService
{
    private readonly SymmetricSecurityKey _jwtSymmetricSecurityKey;
    private const string SecurityAlgorithm = SecurityAlgorithms.HmacSha256;

    public TokenService(IConfiguration configuration) =>
        _jwtSymmetricSecurityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["Jwt:Key"]
                                   ?? throw new ApiException("Jwt:Key is not set in configuration.")));

    public ClaimsPrincipal GetClaimsPrincipal(string bearerToken)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _jwtSymmetricSecurityKey,
            ClockSkew = TimeSpan.Zero
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(bearerToken, tokenValidationParameters, out var securityToken);
        var validToken = securityToken is JwtSecurityToken jwtSecurityToken
                      && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithm, StringComparison.InvariantCultureIgnoreCase);

        if (validToken is false)
            throw new ApiException("Invalid token");

        return principal;
    }

    public TokenModel Create(string userId)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.NameId, userId)
        };

        var expiresIn = DateTime.UtcNow.AddMinutes(1);
        var token = new JwtSecurityToken(
            expires: expiresIn,
            claims: claims,
            signingCredentials: new SigningCredentials(_jwtSymmetricSecurityKey, SecurityAlgorithm)
        );

        return new TokenModel
        {
            BearerToken = new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken = Guid.NewGuid().ToString(),
            ExpiresIn = expiresIn
        };
    }
}