using System;
using System.Collections.Generic;

namespace Disco.Models;

public partial class Artist
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Bio { get; set; }

    public string? Avatar { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public DateTime? Deletedat { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
