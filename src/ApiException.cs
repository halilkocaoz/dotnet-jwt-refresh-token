namespace dotnet_jwt_refresh_token;

public class ApiException : Exception
{
    public ApiException(string message) : base(message)
    {
    }
}