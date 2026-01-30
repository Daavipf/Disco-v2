using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Disco.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "artists",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    bio = table.Column<string>(type: "text", nullable: true),
                    avatar = table.Column<string>(type: "text", nullable: true),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "now()"),
                    updatedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "now()"),
                    deletedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("artists_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true, defaultValueSql: "'USER'::character varying"),
                    hashpassword = table.Column<string>(type: "text", nullable: false),
                    resetpasswordtoken = table.Column<string>(type: "text", nullable: true),
                    bio = table.Column<string>(type: "text", nullable: true),
                    avatar = table.Column<string>(type: "text", nullable: true),
                    isverified = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    verificationtoken = table.Column<string>(type: "text", nullable: true),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "now()"),
                    updatedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "now()"),
                    deletedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("users_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "artists_followers",
                columns: table => new
                {
                    artistid = table.Column<Guid>(type: "uuid", nullable: false),
                    userid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("artists_followers_pkey", x => new { x.artistid, x.userid });
                    table.ForeignKey(
                        name: "artists_followers_artistid_fkey",
                        column: x => x.artistid,
                        principalTable: "artists",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "artists_followers_userid_fkey",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "posts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    authorid = table.Column<Guid>(type: "uuid", nullable: false),
                    artistid = table.Column<Guid>(type: "uuid", nullable: false),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "now()"),
                    updatedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "now()"),
                    deletedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("posts_pkey", x => x.id);
                    table.ForeignKey(
                        name: "posts_artistid_fkey",
                        column: x => x.artistid,
                        principalTable: "artists",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "posts_authorid_fkey",
                        column: x => x.authorid,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "users_followers",
                columns: table => new
                {
                    followedid = table.Column<Guid>(type: "uuid", nullable: false),
                    followerid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("users_followers_pkey", x => new { x.followedid, x.followerid });
                    table.ForeignKey(
                        name: "users_followers_followedid_fkey",
                        column: x => x.followedid,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "users_followers_followerid_fkey",
                        column: x => x.followerid,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "post_reactions",
                columns: table => new
                {
                    postid = table.Column<Guid>(type: "uuid", nullable: false),
                    userid = table.Column<Guid>(type: "uuid", nullable: false),
                    reactiontype = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("post_reactions_pkey", x => new { x.postid, x.userid });
                    table.ForeignKey(
                        name: "post_reactions_postid_fkey",
                        column: x => x.postid,
                        principalTable: "posts",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "post_reactions_userid_fkey",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "replies",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    content = table.Column<string>(type: "text", nullable: false),
                    authorid = table.Column<Guid>(type: "uuid", nullable: false),
                    postid = table.Column<Guid>(type: "uuid", nullable: false),
                    parentid = table.Column<Guid>(type: "uuid", nullable: true),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "now()"),
                    updatedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "now()"),
                    deletedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("replies_pkey", x => x.id);
                    table.ForeignKey(
                        name: "replies_authorid_fkey",
                        column: x => x.authorid,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "replies_parentid_fkey",
                        column: x => x.parentid,
                        principalTable: "replies",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "replies_postid_fkey",
                        column: x => x.postid,
                        principalTable: "posts",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "replies_reactions",
                columns: table => new
                {
                    replyid = table.Column<Guid>(type: "uuid", nullable: false),
                    userid = table.Column<Guid>(type: "uuid", nullable: false),
                    reactiontype = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("replies_reactions_pkey", x => new { x.replyid, x.userid });
                    table.ForeignKey(
                        name: "replies_reactions_replyid_fkey",
                        column: x => x.replyid,
                        principalTable: "replies",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "replies_reactions_userid_fkey",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_artists_followers_userid",
                table: "artists_followers",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "idx_reactions_user",
                table: "post_reactions",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "idx_active_posts",
                table: "posts",
                column: "artistid",
                filter: "(deletedat IS NULL)");

            migrationBuilder.CreateIndex(
                name: "idx_posts_author",
                table: "posts",
                column: "authorid");

            migrationBuilder.CreateIndex(
                name: "idx_replies_parent",
                table: "replies",
                column: "parentid");

            migrationBuilder.CreateIndex(
                name: "idx_replies_post",
                table: "replies",
                column: "postid");

            migrationBuilder.CreateIndex(
                name: "IX_replies_authorid",
                table: "replies",
                column: "authorid");

            migrationBuilder.CreateIndex(
                name: "idx_replies_reactions_user",
                table: "replies_reactions",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "users_email_key",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_followers_followerid",
                table: "users_followers",
                column: "followerid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "artists_followers");

            migrationBuilder.DropTable(
                name: "post_reactions");

            migrationBuilder.DropTable(
                name: "replies_reactions");

            migrationBuilder.DropTable(
                name: "users_followers");

            migrationBuilder.DropTable(
                name: "replies");

            migrationBuilder.DropTable(
                name: "posts");

            migrationBuilder.DropTable(
                name: "artists");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
