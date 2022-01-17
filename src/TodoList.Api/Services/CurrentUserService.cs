using System.Security.Claims;
using TodoList.Application.Common.Interfaces;

namespace TodoList.Api.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserName  => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);
}