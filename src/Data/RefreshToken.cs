namespace dotnet_jwt_refresh_token.Data;

public class RefreshToken
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Token { get; set; } = Guid.NewGuid().ToString();
}