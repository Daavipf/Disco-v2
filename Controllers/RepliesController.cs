using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Disco.Models;
using Disco.DTOs;
using Disco.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Disco.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepliesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RepliesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Replies/Post/{postId}
        [HttpGet("Post/{postId}")]
        public async Task<ActionResult<IEnumerable<ReplyResponseDTO>>> GetRepliesByPost(Guid postId)
        {
            var replies = await _context.Replies
                .Include(r => r.Author)
                .Include(r => r.RepliesReactions)
                .Where(r => r.Postid == postId && r.Deletedat == null)
                .OrderBy(r => r.Createdat)
                .ToListAsync();

            var replyDTOs = replies.Select(MapToDTO).ToList();

            return Ok(replyDTOs);
        }

        // GET: api/Replies/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ReplyResponseDTO>> GetReply(Guid id)
        {
            var reply = await _context.Replies
                .Include(r => r.Author)
                .Include(r => r.RepliesReactions)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reply == null || reply.Deletedat != null)
            {
                return NotFound();
            }

            return Ok(MapToDTO(reply));
        }

        // POST: api/Replies
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ReplyResponseDTO>> PostReply(CreateReplyDTO createDto)
        {
            var userId = User.GetId();

            var postExists = await _context.Posts.AnyAsync(p => p.Id == createDto.PostId);
            if (!postExists)
            {
                return BadRequest("O Post informado não existe.");
            }

            if (createDto.ParentId.HasValue)
            {
                var parentReply = await _context.Replies.FindAsync(createDto.ParentId.Value);

                if (parentReply == null)
                {
                    return BadRequest("A resposta pai (ParentId) informada não existe.");
                }

                // REGRA CRÍTICA: Uma resposta não pode estar associada a um Post diferente do seu Pai.
                // Isso evita que uma thread "pule" de um post para outro.
                if (parentReply.Postid != createDto.PostId)
                {
                    return BadRequest("Inconsistência: A resposta pai pertence a um post diferente.");
                }
            }

            var reply = new Reply
            {
                Id = Guid.NewGuid(),
                Content = createDto.Content,
                Authorid = userId,
                Postid = createDto.PostId,
                Parentid = createDto.ParentId,
                Createdat = DateTime.UtcNow,
                RepliesReactions = new List<RepliesReaction>()
            };

            _context.Replies.Add(reply);
            await _context.SaveChangesAsync();

            await _context.Entry(reply).Reference(r => r.Author).LoadAsync();

            return CreatedAtAction(nameof(GetReply), new { id = reply.Id }, MapToDTO(reply));
        }

        // PUT: api/Replies/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReply(Guid id, UpdateReplyDTO updateDto)
        {
            var userId = User.GetId();
            var reply = await _context.Replies.FindAsync(id);

            if (reply == null || reply.Deletedat != null)
            {
                return NotFound();
            }

            if (reply.Authorid != userId)
            {
                return StatusCode(403, "Você não tem permissão para editar esta resposta.");
            }

            reply.Content = updateDto.Content;
            reply.Updatedat = DateTime.UtcNow;

            _context.Entry(reply).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReplyExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        [Authorize]
        [HttpPost("react")]
        public async Task<IActionResult> ReactToReply(ReplyReactionDTO reactionDto)
        {
            var userId = User.GetId();

            var replyExists = await _context.Replies.AnyAsync(r => r.Id == reactionDto.ReplyId);
            if (!replyExists)
            {
                return NotFound("Resposta não encontrada.");
            }

            var existingReaction = await _context.RepliesReactions
                .FirstOrDefaultAsync(r => r.Replyid == reactionDto.ReplyId && r.Userid == userId);

            if (existingReaction != null)
            {
                if (existingReaction.Reactiontype == reactionDto.ReactionType)
                {
                    _context.RepliesReactions.Remove(existingReaction);
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

            var newReaction = new RepliesReaction
            {
                Replyid = reactionDto.ReplyId,
                Userid = userId,
                Reactiontype = reactionDto.ReactionType
            };

            _context.RepliesReactions.Add(newReaction);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Reação adicionada com sucesso." });
        }

        // DELETE: api/Replies/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReply(Guid id)
        {
            var reply = await _context.Replies.FindAsync(id);
            if (reply == null)
            {
                return NotFound();
            }

            reply.Deletedat = DateTime.UtcNow;
            reply.Content = "[Removido pelo usuário]";

            _context.Replies.Update(reply);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ReplyExists(Guid id)
        {
            return _context.Replies.Any(e => e.Id == id);
        }

        private static ReplyResponseDTO MapToDTO(Reply reply)
        {
            return new ReplyResponseDTO
            {
                Id = reply.Id,
                Content = reply.Content,
                AuthorId = reply.Authorid,
                AuthorName = reply.Author?.Name ?? "Desconhecido",
                PostId = reply.Postid,
                ParentId = reply.Parentid,
                CreatedAt = reply.Createdat ?? DateTime.MinValue,
                UpdatedAt = reply.Updatedat,
                Reactions = reply.RepliesReactions
                    .GroupBy(r => r.Reactiontype)
                    .ToDictionary(g => g.Key, g => g.Count())
            };
        }
    }
}