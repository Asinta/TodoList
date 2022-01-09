using Microsoft.AspNetCore.Mvc;
using TodoList.Application.Common.Interfaces;
using TodoList.Application.Common.Models;

namespace TodoList.Api.Controllers;

[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(IIdentityService identityService, ILogger<AuthenticationController> logger)
    {
        _identityService = identityService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Authenticate([FromBody] UserForAuthentication userForAuthentication)
    {
        if (!await _identityService.ValidateUserAsync(userForAuthentication))
        {
            return Unauthorized();
        }
        
        var token = await _identityService.CreateTokenAsync(true);
        return Ok(token);
    }
    
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] ApplicationToken token)
    {
        var tokenToReturn = await _identityService.RefreshTokenAsync(token);
        
        return Ok(tokenToReturn);
    }
}