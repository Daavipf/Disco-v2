using System;
using System.Collections.Generic;

namespace Disco.Models;

public partial class Reply
{
    public Guid Id { get; set; }

    public string Content { get; set; } = null!;

    public Guid Authorid { get; set; }

    public Guid Postid { get; set; }

    public Guid? Parentid { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public DateTime? Deletedat { get; set; }

    public virtual User Author { get; set; } = null!;

    public virtual ICollection<Reply> InverseParent { get; set; } = new List<Reply>();

    public virtual Reply? Parent { get; set; }

    public virtual Post Post { get; set; } = null!;

    public virtual ICollection<RepliesReaction> RepliesReactions { get; set; } = new List<RepliesReaction>();
}
