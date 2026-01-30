using System.ComponentModel.DataAnnotations;

namespace Disco.DTOs;

public class ForgotPasswordDTO
{
  [Required]
  [EmailAddress]
  public string Email { get; set; } = string.Empty;

}