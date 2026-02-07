using System.ComponentModel.DataAnnotations;
using Disco.Models;

namespace Disco.DTOs;

public class UserDTO
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;

  [EmailAddress]
  public string Email { get; set; } = string.Empty;
  public bool? Isverified { get; set; } = false;
  public DateTime? Createdat { get; set; }
  public DateTime? Updatedat { get; set; }
  public UserRole? Role { get; set; }
  public string? Bio { get; set; }
  public string? Avatar { get; set; }
}