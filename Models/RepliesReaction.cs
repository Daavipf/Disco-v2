using System;
using System.Collections.Generic;

namespace Disco.Models;

public partial class RepliesReaction
{
    public Guid Replyid { get; set; }

    public Guid Userid { get; set; }

    public string Reactiontype { get; set; } = null!;

    public virtual Reply Reply { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
