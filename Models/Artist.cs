using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace Disco.Models;

public partial class Artist
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Bio { get; set; }

    public string? Avatar { get; set; }

    [Column(TypeName = "timestamp with time zone")]
    public DateTime? Createdat { get; set; }

    [Column(TypeName = "timestamp with time zone")]
    public DateTime? Updatedat { get; set; }

    [Column(TypeName = "timestamp with time zone")]
    public DateTime? Deletedat { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
