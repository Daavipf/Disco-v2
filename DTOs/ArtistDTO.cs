using System.ComponentModel.DataAnnotations;

namespace Disco.DTOs;

public class ArtistRequestDTO
{
  [Required]
  public string Name { get; set; } = null!;

  [Required]
  public string? Bio { get; set; }

  [Required]
  public string? Avatar { get; set; }
}

public class ArtistResponseDTO
{
  public Guid Id { get; set; }
  public string Name { get; set; } = null!;
  public string? Bio { get; set; }
  public string? Avatar { get; set; }
  public DateTime? Createdat { get; set; }
  public DateTime? Updatedat { get; set; }
}