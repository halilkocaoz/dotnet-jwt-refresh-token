namespace dotnet_jwt_refresh_token.Models;

public class TokenModel
{
    public string BearerToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiresIn { get; set; }
}