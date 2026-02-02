using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Disco.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Artist> Artists { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<PostReaction> PostReactions { get; set; }

    public virtual DbSet<RepliesReaction> RepliesReactions { get; set; }

    public virtual DbSet<Reply> Replies { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // if (!optionsBuilder.IsConfigured)
        // {
        //     optionsBuilder.UseNpgsql("Host=localhost;Database=disco;Username=davi;Password=2705");
        // }
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Artist>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("artists_pkey");

            entity.ToTable("artists");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Avatar).HasColumnName("avatar");
            entity.Property(e => e.Bio).HasColumnName("bio");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Deletedat)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("deletedat");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updatedat");

            entity.HasMany(d => d.Users).WithMany(p => p.Artists)
                .UsingEntity<Dictionary<string, object>>(
                    "ArtistsFollower",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("Userid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("artists_followers_userid_fkey"),
                    l => l.HasOne<Artist>().WithMany()
                        .HasForeignKey("Artistid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("artists_followers_artistid_fkey"),
                    j =>
                    {
                        j.HasKey("Artistid", "Userid").HasName("artists_followers_pkey");
                        j.ToTable("artists_followers");
                        j.IndexerProperty<Guid>("Artistid").HasColumnName("artistid");
                        j.IndexerProperty<Guid>("Userid").HasColumnName("userid");
                    });
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("posts_pkey");

            entity.ToTable("posts");

            entity.HasIndex(e => e.Artistid, "idx_active_posts").HasFilter("(deletedat IS NULL)");

            entity.HasIndex(e => e.Authorid, "idx_posts_author");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Artistid).HasColumnName("artistid");
            entity.Property(e => e.Authorid).HasColumnName("authorid");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Deletedat)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("deletedat");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updatedat");

            entity.HasOne(d => d.Artist).WithMany(p => p.Posts)
                .HasForeignKey(d => d.Artistid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("posts_artistid_fkey");

            entity.HasOne(d => d.Author).WithMany(p => p.Posts)
                .HasForeignKey(d => d.Authorid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("posts_authorid_fkey");
        });

        modelBuilder.Entity<PostReaction>(entity =>
        {
            entity.HasKey(e => new { e.Postid, e.Userid }).HasName("post_reactions_pkey");

            entity.ToTable("post_reactions");

            entity.HasIndex(e => e.Userid, "idx_reactions_user");

            entity.Property(e => e.Postid).HasColumnName("postid");
            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Reactiontype)
                .HasMaxLength(20)
                .HasColumnName("reactiontype");

            entity.HasOne(d => d.Post).WithMany(p => p.PostReactions)
                .HasForeignKey(d => d.Postid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("post_reactions_postid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.PostReactions)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("post_reactions_userid_fkey");
        });

        modelBuilder.Entity<RepliesReaction>(entity =>
        {
            entity.HasKey(e => new { e.Replyid, e.Userid }).HasName("replies_reactions_pkey");

            entity.ToTable("replies_reactions");

            entity.HasIndex(e => e.Userid, "idx_replies_reactions_user");

            entity.Property(e => e.Replyid).HasColumnName("replyid");
            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Reactiontype)
                .HasMaxLength(20)
                .HasColumnName("reactiontype");

            entity.HasOne(d => d.Reply).WithMany(p => p.RepliesReactions)
                .HasForeignKey(d => d.Replyid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("replies_reactions_replyid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.RepliesReactions)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("replies_reactions_userid_fkey");
        });

        modelBuilder.Entity<Reply>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("replies_pkey");

            entity.ToTable("replies");

            entity.HasIndex(e => e.Parentid, "idx_replies_parent");

            entity.HasIndex(e => e.Postid, "idx_replies_post");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Authorid).HasColumnName("authorid");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Deletedat)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("deletedat");
            entity.Property(e => e.Parentid).HasColumnName("parentid");
            entity.Property(e => e.Postid).HasColumnName("postid");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updatedat");

            entity.HasOne(d => d.Author).WithMany(p => p.Replies)
                .HasForeignKey(d => d.Authorid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("replies_authorid_fkey");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.Parentid)
                .HasConstraintName("replies_parentid_fkey");

            entity.HasOne(d => d.Post).WithMany(p => p.Replies)
                .HasForeignKey(d => d.Postid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("replies_postid_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Avatar).HasColumnName("avatar");
            entity.Property(e => e.Bio).HasColumnName("bio");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Deletedat)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("deletedat");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Hashpassword).HasColumnName("hashpassword");
            entity.Property(e => e.Isverified)
                .HasDefaultValue(false)
                .HasColumnName("isverified");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Resetpasswordtoken).HasColumnName("resetpasswordtoken");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasColumnName("role")
                .HasConversion<string>();
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updatedat");
            entity.Property(e => e.Verificationtoken).HasColumnName("verificationtoken");

            entity.HasMany(d => d.Followeds).WithMany(p => p.Followers)
                .UsingEntity<Dictionary<string, object>>(
                    "UsersFollower",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("Followedid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("users_followers_followedid_fkey"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("Followerid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("users_followers_followerid_fkey"),
                    j =>
                    {
                        j.HasKey("Followedid", "Followerid").HasName("users_followers_pkey");
                        j.ToTable("users_followers");
                        j.IndexerProperty<Guid>("Followedid").HasColumnName("followedid");
                        j.IndexerProperty<Guid>("Followerid").HasColumnName("followerid");
                    });

            entity.HasMany(d => d.Followers).WithMany(p => p.Followeds)
                .UsingEntity<Dictionary<string, object>>(
                    "UsersFollower",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("Followerid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("users_followers_followerid_fkey"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("Followedid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("users_followers_followedid_fkey"),
                    j =>
                    {
                        j.HasKey("Followedid", "Followerid").HasName("users_followers_pkey");
                        j.ToTable("users_followers");
                        j.IndexerProperty<Guid>("Followedid").HasColumnName("followedid");
                        j.IndexerProperty<Guid>("Followerid").HasColumnName("followerid");
                    });
        });

        modelBuilder.Entity<Post>().HasQueryFilter(p => p.Deletedat == null);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is User && e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            ((User)entry.Entity).Updatedat = DateTime.UtcNow;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
