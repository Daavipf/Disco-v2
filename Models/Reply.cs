using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace Disco.Models;

public partial class Reply
{
    public Guid Id { get; set; }

    public string Content { get; set; } = null!;

    public Guid Authorid { get; set; }

    public Guid Postid { get; set; }

    public Guid? Parentid { get; set; }

    [Column(TypeName = "timestamp with time zone")]
    public DateTime? Createdat { get; set; }

    [Column(TypeName = "timestamp with time zone")]
    public DateTime? Updatedat { get; set; }

    [Column(TypeName = "timestamp with time zone")]
    public DateTime? Deletedat { get; set; }

    public virtual User Author { get; set; } = null!;

    public virtual ICollection<Reply> InverseParent { get; set; } = new List<Reply>();

    public virtual Reply? Parent { get; set; }

    public virtual Post Post { get; set; } = null!;

    public virtual ICollection<RepliesReaction> RepliesReactions { get; set; } = new List<RepliesReaction>();
}
