using System.ComponentModel.DataAnnotations;

namespace Disco.DTOs;

public class PostResponseDTO
{
  public Guid Id { get; set; }
  public Guid AuthorId { get; set; }
  public Guid ArtistId { get; set; }
  public string Title { get; set; } = string.Empty;
  public string Content { get; set; } = string.Empty;
  public Dictionary<string, int> Reactions { get; set; } = new Dictionary<string, int>();

}