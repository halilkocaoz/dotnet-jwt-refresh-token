using dotnet_jwt_refresh_token.Data;
using dotnet_jwt_refresh_token.Models;
using dotnet_jwt_refresh_token.Services;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_jwt_refresh_token.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthenticateController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthenticateController(IUserService userService, ITokenService tokenService, IRefreshTokenService refreshTokenService, IHttpContextAccessor httpContextAccessor)
    {
        _userService = userService;
        _tokenService = tokenService;
        _refreshTokenService = refreshTokenService;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpPost]
    public async Task<TokenModel> AuthenticateAsync([FromBody] AuthenticateUserRequest request)
    {
        var user = await _userService.GetUserByCredentialsAsync(request);
        if (user is null)
        {
            throw new ApiException("Invalid credentials.");
        }
        var token = _tokenService.Create(user.Id.ToString());
        await _refreshTokenService.AddAsync(new RefreshToken()
        {
            UserId = user.Id.ToString(),
            Token = token.RefreshToken,
        });
        await _refreshTokenService.SaveChangesAsync();
        return token;
    }

    [HttpPost("refresh")]
    public async Task<TokenModel> RefreshTokenAsync([FromBody] RefreshTokenRequest request)
    {
        _httpContextAccessor.HttpContext?.Request.Headers.TryGetValue("Authorization", out var authorizationValue);
        var token = authorizationValue.ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
        {
            throw new ApiException("Authorization header is missing or value is empty.");
        }
        var claimsPrincipal = _tokenService.GetClaimsPrincipal(token);
        var userId = claimsPrincipal.GetId();

        var activeRefreshToken = await _refreshTokenService.GetAsync(userId, request.ActiveRefreshToken);
        if (activeRefreshToken == null)
        {
            throw new ApiException("Invalid refresh token.");
        }

        var newToken = _tokenService.Create(userId);
        await _refreshTokenService.DeleteAsync(activeRefreshToken);
        await _refreshTokenService.AddAsync(new RefreshToken
        {
            Token = newToken.RefreshToken,
            UserId = userId
        });
        await _refreshTokenService.SaveChangesAsync();

        return newToken;
    }
}