namespace dotnet_jwt_refresh_token;

public class UnhandledExceptionsMiddleware
{
    private readonly RequestDelegate _next;

    public UnhandledExceptionsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (ApiException apiException)
        {
            httpContext.Response.StatusCode = 400;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsJsonAsync(apiException.Message);
        }
        catch (Exception e)
        {
            httpContext.Response.StatusCode = 500;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsJsonAsync($"Internal server error: {e.Message}");
        }
    }
}