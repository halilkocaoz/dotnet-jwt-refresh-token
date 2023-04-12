namespace dotnet_jwt_refresh_token.Models;

public class AuthenticateUserRequest
{
    public string UserName { get; set; }
    public string Password { get; set; }
}