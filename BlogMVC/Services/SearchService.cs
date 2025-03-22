using BlogMVC.Data;
using BlogMVC.Models;
using BlogMVC.Models.ViewModels;
using BlogMVC.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogMVC.Services {
    public class SearchService : ISearchService {

        private readonly BlogNowContext _context;

        public SearchService(BlogNowContext context) {
            _context = context;
        }

        public async Task<PageResult<User>> GetUsersPage(string? text, uint? page) {

            int skip = (int)((page - 1) * 10);
            int totalItems = await _context.Users.Where(u => u.NickName.Contains(text) || u.Email.Contains(text)).CountAsync();
            uint totalPages = (uint)Math.Ceiling((double)totalItems / 10);
            var posts = await _context.Users.Where(u => u.NickName.Contains(text) || u.Email.Contains(text)).AsNoTracking().Skip(skip).Take(10).Include(u => u.Following).Include(u => u.Followed).ToListAsync();
            return new PageResult<User>() {
                Items = posts,
                TotalPages = totalPages,
                CurrentPage = (uint)page
            };
        }

        public async Task<PageResult<Post>> GetPostsPage(string? text, uint? page) {

            int skip = (int)((page - 1) * 10);
            int totalItems = await _context.Posts.Where(p => p.Message.Contains(text)).CountAsync();
            uint totalPages = (uint)Math.Ceiling((double)totalItems / 10);
            var posts = await _context.Posts.Where(p => p.Message.Contains(text)).AsNoTracking().Skip(skip).Take(10).ToListAsync();
            return new PageResult<Post>() {
                Items = posts,
                TotalPages = totalPages,
                CurrentPage = (uint)page
            };
        }
    }
}
