using System.ComponentModel.DataAnnotations;

namespace Disco.DTOs;

public class PostRequestDTO
{
  [Required]
  public string Title { get; set; } = string.Empty;

  [Required]
  public string Content { get; set; } = string.Empty;

  [Required]
  public Guid Artistid { get; set; }
}