using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TodoList.Application.Common.Configurations;
using TodoList.Application.Common.Interfaces;
using TodoList.Application.Common.Models;

namespace TodoList.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly ILogger<IdentityService> _logger;

    private readonly UserManager<ApplicationUser> _userManager;
    
    private readonly JwtConfiguration _jwtConfiguration;
    
    private ApplicationUser? User;

    public IdentityService(
        ILogger<IdentityService> logger,
        UserManager<ApplicationUser> userManager,
        IOptionsMonitor<JwtConfiguration> jwtOptions)
    {
        _logger = logger;
        _userManager = userManager;

        // 使用IOptionsMonitor加载配置
        _jwtConfiguration = jwtOptions.CurrentValue;
    }

    public async Task<string> CreateUserAsync(string userName, string password)
    {
        var user = new ApplicationUser
        {
            UserName = userName,
            Email = userName
        };

        await _userManager.CreateAsync(user, password);

        return user.Id;
    }

    public async Task<bool> ValidateUserAsync(UserForAuthentication userForAuthentication)
    {
        User = await _userManager.FindByNameAsync(userForAuthentication.UserName);
        
        var result = User != null && await _userManager.CheckPasswordAsync(User, userForAuthentication.Password);
        if (!result)
        {
            _logger.LogWarning($"{nameof(ValidateUserAsync)}: Authentication failed. Wrong username or password.");
        }
        
        return result;
    }

    public async Task<ApplicationToken> CreateTokenAsync(bool populateExpiry)
    {
        var signingCredentials = GetSigningCredentials();
        var claims = await GetClaims();
        var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
        
        var refreshToken = GenerateRefreshToken();
        
        User!.RefreshToken = refreshToken;
        if(populateExpiry)
            User!.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
        
        await _userManager.UpdateAsync(User);
        
        var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        
        return new ApplicationToken(accessToken, refreshToken);
    }

    public async Task<ApplicationToken> RefreshTokenAsync(ApplicationToken token)
    {
        var principal = GetPrincipalFromExpiredToken(token.AccessToken);
        
        var user = await _userManager.FindByNameAsync(principal.Identity?.Name);
        if (user == null || user.RefreshToken != token.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
        {
            throw new BadHttpRequestException("provided token has some invalid value");
        }
        
        User = user;
        return await CreateTokenAsync(true);
    }
    
    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET") ?? "TodoListApiSecretKey")), ValidateLifetime = true,
            // 更改为通过类对象获取
            ValidIssuer = _jwtConfiguration.ValidIssuer,
            ValidAudience = _jwtConfiguration.ValidAudience
        };
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken || 
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }
        
        return principal;
    }

    private string GenerateRefreshToken()
    {
        // 创建一个随机的Token用做Refresh Token
        var randomNumber = new byte[32];
        
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        
        return Convert.ToBase64String(randomNumber);
    }

    private SigningCredentials GetSigningCredentials()
    {
        // 出于演示的目的，我将SECRET值在这里fallback成了硬编码的字符串，实际环境中，最好是需要从环境变量中进行获取，而不应该写在代码中
        var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET") ?? "TodoListApiSecretKey");
        var secret = new SymmetricSecurityKey(key);
        
        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    private async Task<List<Claim>> GetClaims()
    {
        // 演示了返回用户名和Role两个claims
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, User!.UserName),
            new(JwtRegisteredClaimNames.Iss, _jwtConfiguration.ValidIssuer ?? "TodoListApi"),
            new(JwtRegisteredClaimNames.Aud, _jwtConfiguration.ValidAudience ?? "https://localhost:5050")
        };
        
        var roles = await _userManager.GetRolesAsync(User);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        
        return claims;
    }
    
    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
    {
        // 配置JWT选项
        var tokenOptions = new JwtSecurityToken
        (
            _jwtConfiguration.ValidIssuer,
            _jwtConfiguration.ValidAudience,
            claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(_jwtConfiguration.Expires)),
            signingCredentials: signingCredentials
        );
        return tokenOptions;
    }
}