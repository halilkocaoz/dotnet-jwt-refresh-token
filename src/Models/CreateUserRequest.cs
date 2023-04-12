namespace dotnet_jwt_refresh_token.Models;

public class CreateUserRequest
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
}