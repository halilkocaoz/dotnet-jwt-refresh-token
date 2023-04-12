using dotnet_jwt_refresh_token.Data;
using dotnet_jwt_refresh_token.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_jwt_refresh_token.Services;

public interface IUserService
{
    /// <summary>
    /// Creates a new user
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task CreateAsync(CreateUserRequest request);

    /// <summary>
    /// Gets user by credential information
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Token model or throws exception</returns>
    Task<User> GetUserByCredentialsAsync(AuthenticateUserRequest request);
}

public class UserService : IUserService
{
    private readonly DatabaseContext _databaseContext;

    public UserService(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task CreateAsync(CreateUserRequest request)
    {
        var anyUserWithSameUserName = await _databaseContext.Users.AnyAsync(x => x.UserName == request.UserName);
        if (anyUserWithSameUserName)
            throw new ApiException($"User with same username({request.UserName}) already exist.");

        await _databaseContext.Users.AddAsync(new User
        {
            UserName = request.UserName,
            Password = request.Password
        });

        await _databaseContext.SaveChangesAsync();
    }

    public async Task<User> GetUserByCredentialsAsync(AuthenticateUserRequest request)
    {
        return await _databaseContext.Users.FirstOrDefaultAsync(x => x.UserName == request.UserName
                                                                     && x.Password == request.Password);
    }
}