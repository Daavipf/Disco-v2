using System.ComponentModel.DataAnnotations;

namespace Disco.DTOs;

public class ReactionDTO
{
  [Required]
  public Guid PostId { get; set; }

  [Required]
  public string ReactionType { get; set; } = null!;
}