using System.ComponentModel.DataAnnotations;

namespace TodoList.Application.Common.Models;

public record UserForAuthentication
{
    [Required(ErrorMessage = "username is required")]
    public string? UserName { get; set; }
    
    [Required(ErrorMessage = "password is required")]
    public string? Password { get; set; }
}