using api.Data;
using api.Dtos.Comment;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _context;

        public CommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Comment> CreateAsync(Comment commentModel)
        {
            await _context.Comments.AddAsync(commentModel);
            await _context.SaveChangesAsync();
            return commentModel;
        }

        public async Task<Comment?> DeleteAsync(int id)
        {
            var commentModel = await _context.Comments.FirstOrDefaultAsync(x => x.Id == id);
            if (commentModel == null)
            {
                return null;
            }

            _context.Comments.Remove(commentModel);
            await _context.SaveChangesAsync();

            return commentModel;
        }

        public async Task<List<Comment>> GetAllAsync() => await _context.Comments.ToListAsync();

        public async Task<Comment?> GetByIdAsync(int id) => await _context.Comments.FindAsync(id);

        public async Task<Comment?> UpdateAsync(int id, UpdateCommentDto updateCommentDto)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(x => x.Id == id);

            if (comment == null)
            {
                return null;
            }

            comment.Title = updateCommentDto.Title;
            comment.Content = updateCommentDto.Content;

            await _context.SaveChangesAsync();

            return comment;
        }
    }
}
