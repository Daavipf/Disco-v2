using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Disco.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public UserRole Role { get; set; } = UserRole.User;

    public string Hashpassword { get; set; } = null!;

    public string? Resetpasswordtoken { get; set; }

    public string? Bio { get; set; }

    public string? Avatar { get; set; }

    public bool Isverified { get; set; } = false;

    public string? Verificationtoken { get; set; }

    [Column(TypeName = "timestamp with time zone")]
    public DateTime? Createdat { get; set; }

    [Column(TypeName = "timestamp with time zone")]
    public DateTime? Updatedat { get; set; }

    [Column(TypeName = "timestamp with time zone")]
    public DateTime? Deletedat { get; set; }

    public virtual ICollection<PostReaction> PostReactions { get; set; } = new List<PostReaction>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<Reply> Replies { get; set; } = new List<Reply>();

    public virtual ICollection<RepliesReaction> RepliesReactions { get; set; } = new List<RepliesReaction>();

    public virtual ICollection<Artist> Artists { get; set; } = new List<Artist>();

    public virtual ICollection<User> Followeds { get; set; } = new List<User>();

    public virtual ICollection<User> Followers { get; set; } = new List<User>();
}
