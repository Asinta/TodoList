namespace TodoList.Application.Common.Configurations;

public class JwtConfiguration
{
    public string Section { get; set; } = "JwtSettings";
    public string? ValidIssuer { get; set; }
    public string? ValidAudience { get; set; }
    public string? Expires { get; set; }
}