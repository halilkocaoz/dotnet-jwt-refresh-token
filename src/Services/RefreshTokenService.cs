using dotnet_jwt_refresh_token.Data;
using Microsoft.EntityFrameworkCore;

namespace dotnet_jwt_refresh_token.Services;

public interface IRefreshTokenService
{
    /// <summary>
    /// Creates new refresh token.
    /// </summary>
    Task AddAsync(RefreshToken refreshToken);

    /// <summary>
    /// Gets the refresh token.
    /// </summary>
    Task<RefreshToken> GetAsync(string userId, string refreshTokenValue);

    /// <summary>
    /// Deletes the refresh token.
    /// </summary>
    public Task DeleteAsync(RefreshToken refreshToken);

    /// <summary>
    /// Saves the changes.
    /// </summary>
    Task<int> SaveChangesAsync();
}

public class RefreshTokenService : IRefreshTokenService
{
    private readonly DatabaseContext _databaseContext;

    public RefreshTokenService(DatabaseContext databaseContext)
        => _databaseContext = databaseContext;

    public async Task AddAsync(RefreshToken refreshToken)
        => await _databaseContext.RefreshTokens.AddAsync(refreshToken);

    public async Task<RefreshToken> GetAsync(string userId, string refreshTokenValue)
        => await _databaseContext.RefreshTokens
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Token == refreshTokenValue);

    public Task DeleteAsync(RefreshToken refreshToken)
    {
        if (refreshToken is not null)
            _databaseContext.Remove(refreshToken);

        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync()
        => await _databaseContext.SaveChangesAsync();
}