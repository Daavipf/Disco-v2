using System.ComponentModel.DataAnnotations;

namespace Disco.DTOs;

public class ResetPasswordDTO
{
    [Required]
    public string Token { get; set; } = string.Empty;

    [Required]
    [StringLength(20, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare("Password", ErrorMessage = "As senhas não conferem")]
    public string ConfirmPassword { get; set; } = string.Empty;
}