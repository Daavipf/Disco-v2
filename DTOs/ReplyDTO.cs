using System.ComponentModel.DataAnnotations;

namespace Disco.DTOs;

public class CreateReplyDTO
{
    [Required]
    public string Content { get; set; } = string.Empty;

    [Required]
    public Guid PostId { get; set; }

    public Guid? ParentId { get; set; }
}

public class UpdateReplyDTO
{
    [Required]
    public string Content { get; set; } = string.Empty;
}

public class ReplyResponseDTO
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public Guid PostId { get; set; }
    public Guid? ParentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Dictionary<string, int> Reactions { get; set; } = new Dictionary<string, int>();
}

public class ReplyReactionDTO
{
    public Guid ReplyId { get; set; }
    public string ReactionType { get; set; } = string.Empty;
}