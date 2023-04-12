using dotnet_jwt_refresh_token.Models;
using dotnet_jwt_refresh_token.Services;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_jwt_refresh_token.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<object> CreateUser(CreateUserRequest request)
    {
        await _userService.CreateAsync(request);
        return StatusCode(201);
    }
}