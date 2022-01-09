using TodoList.Application.Common.Models;

namespace TodoList.Application.Common.Interfaces;

public interface IIdentityService
{
    // 出于演示的目的，只定义以下方法，实际使用的认证服务会提供更多的方法
    Task<string> CreateUserAsync(string userName, string password);
    Task<bool> ValidateUserAsync(UserForAuthentication userForAuthentication);
    Task<ApplicationToken> CreateTokenAsync(bool populateExpiry);
    Task<ApplicationToken> RefreshTokenAsync(ApplicationToken token);
}