using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Disco.Models;
using Disco.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Disco.Extensions;

namespace Disco.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PostsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Posts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostResponseDTO>>> GetPosts()
        {
            var postsData = await _context.Posts
                .AsNoTracking()
                .Select(p => new
                {
                    p.Id,
                    p.Authorid,
                    p.Artistid,
                    p.Title,
                    p.Content,
                    ReactionCounts = p.PostReactions
                        .GroupBy(r => r.Reactiontype)
                        .Select(g => new { Type = g.Key, Count = g.Count() })
                })
                .ToListAsync();

            var result = postsData.Select(p => new PostResponseDTO
            {
                Id = p.Id,
                AuthorId = p.Authorid,
                ArtistId = p.Artistid,
                Title = p.Title,
                Content = p.Content,
                Reactions = p.ReactionCounts.ToDictionary(k => k.Type, v => v.Count)
            });

            return Ok(result);
        }

        // GET: api/Posts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PostResponseDTO>> GetPost(Guid id)
        {
            var postData = await _context.Posts
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new
                {
                    p.Id,
                    p.Authorid,
                    p.Artistid,
                    p.Title,
                    p.Content,
                    ReactionCounts = p.PostReactions
                        .GroupBy(r => r.Reactiontype)
                        .Select(g => new { Type = g.Key, Count = g.Count() })
                })
                .FirstOrDefaultAsync();

            if (postData == null)
            {
                return NotFound();
            }

            var dto = new PostResponseDTO
            {
                Id = postData.Id,
                AuthorId = postData.Authorid,
                ArtistId = postData.Artistid,
                Title = postData.Title,
                Content = postData.Content,
                Reactions = postData.ReactionCounts.ToDictionary(k => k.Type, v => v.Count)
            };

            return Ok(dto);
        }

        // PUT: api/Posts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost(Guid id, PostRequestDTO postDto)
        {
            var userId = User.GetId();

            var existingPost = await _context.Posts.FindAsync(id);

            if (existingPost == null)
            {
                return NotFound("Post não encontrado.");
            }

            if (existingPost.Authorid != userId)
            {
                return Forbid();
            }

            existingPost.Title = postDto.Title;
            existingPost.Content = postDto.Content;
            existingPost.Artistid = postDto.Artistid;

            existingPost.Updatedat = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Posts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Post>> PostPost(PostRequestDTO postDto)
        {
            var userId = User.GetId();

            var post = new Post
            {
                Title = postDto.Title,
                Content = postDto.Content,
                Artistid = postDto.Artistid,
                Authorid = userId,
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPost", new { id = post.Id }, post);
        }

        // HARD-DELETE: api/Posts/{id}/hard
        [Authorize(Roles = nameof(UserRole.Admin))]
        [HttpDelete("{id}/hard")]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            var post = await _context.Posts
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // SOFT-DELETE: api/Posts/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeletePost(Guid id)
        {
            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            var userId = User.GetId();
            if (post.Authorid != userId)
            {
                return Forbid();
            }

            post.Deletedat = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // RESTORE: api/Posts/{id}/restore
        [Authorize]
        [HttpPatch("{id}/restore")]
        public async Task<IActionResult> RestorePost(Guid id)
        {
            var post = await _context.Posts
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == id);
            if (post == null)
            {
                return NotFound("Post não encontrado ou não existe.");
            }

            if (post.Deletedat == null)
            {
                return BadRequest("Este post não está deletado.");
            }

            var userId = User.GetId();
            if (post.Authorid != userId)
            {
                return Forbid();
            }

            post.Deletedat = null;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Post restaurado com sucesso." });
        }

        [Authorize]
        [HttpPost("react")]
        public async Task<IActionResult> ReactToPost(ReactionDTO reactionDto)
        {
            var userId = User.GetId();

            var postExists = await _context.Posts.AnyAsync(p => p.Id == reactionDto.PostId);
            if (!postExists) return NotFound("Post não encontrado.");

            var existingReaction = await _context.PostReactions
                .FirstOrDefaultAsync(r => r.Postid == reactionDto.PostId && r.Userid == userId);

            if (existingReaction != null)
            {
                if (existingReaction.Reactiontype == reactionDto.ReactionType)
                {
                    _context.PostReactions.Remove(existingReaction);
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "Reação removida." });
                }

                else
                {
                    existingReaction.Reactiontype = reactionDto.ReactionType;
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "Reação atualizada." });
                }
            }

            var newReaction = new PostReaction
            {
                Postid = reactionDto.PostId,
                Userid = userId,
                Reactiontype = reactionDto.ReactionType
            };

            _context.PostReactions.Add(newReaction);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Reação adicionada com sucesso." });
        }

        private bool PostExists(Guid id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
