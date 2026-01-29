using System;
using System.Collections.Generic;

namespace Disco.Models;

public partial class PostReaction
{
    public Guid Postid { get; set; }

    public Guid Userid { get; set; }

    public string Reactiontype { get; set; } = null!;

    public virtual Post Post { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
