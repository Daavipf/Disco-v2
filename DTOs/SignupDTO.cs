using System.ComponentModel.DataAnnotations;

namespace Disco.DTOs;

public class SignupDTO
{
  [Required]
  public string Username { get; set; } = string.Empty;

  [Required]
  [EmailAddress]
  public string Email { get; set; } = string.Empty;

  [Required]
  [StringLength(20, MinimumLength = 6)]
  public string Password { get; set; } = string.Empty;

  [Required]
  [Compare("Password", ErrorMessage = "As senhas não conferem")]
  public string ConfirmPassword { get; set; } = string.Empty;
}