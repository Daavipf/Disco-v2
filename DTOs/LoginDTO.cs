using System.ComponentModel.DataAnnotations;

namespace Disco.DTOs;

public class LoginDTO
{
  [Required]
  [EmailAddress]
  public string Email { get; set; } = string.Empty;

  [Required]
  [StringLength(20, MinimumLength = 6)]
  public string Password { get; set; } = string.Empty;
}

public class LoginResponseDTO
{
  public UserDTO User { get; set; }
  public string Token { get; set; } = string.Empty;
}