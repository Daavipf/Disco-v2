using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Disco.Models;

public partial class Post
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public Guid Authorid { get; set; }

    public Guid Artistid { get; set; }

    [Column(TypeName = "timestamp with time zone")]
    public DateTime? Createdat { get; set; }

    [Column(TypeName = "timestamp with time zone")]
    public DateTime? Updatedat { get; set; }

    [Column(TypeName = "timestamp with time zone")]
    public DateTime? Deletedat { get; set; }

    public virtual Artist Artist { get; set; } = null!;

    public virtual User Author { get; set; } = null!;

    public virtual ICollection<PostReaction> PostReactions { get; set; } = new List<PostReaction>();

    public virtual ICollection<Reply> Replies { get; set; } = new List<Reply>();
}
