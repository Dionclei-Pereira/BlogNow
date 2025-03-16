using BlogMVC.Data;
using BlogMVC.Interfaces;
using BlogMVC.Models;
using BlogMVC.Models.Exceptions;
using BlogMVC.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BlogMVC.Services {
    public class PostService : IPostService {

        private readonly BlogNowContext _context;
        public PostService(BlogNowContext context) { 
            _context = context;
        }

        public async Task<List<Post>> GetAllPosts() {
            return await _context.Posts.Include(p => p.likedpeople).AsNoTracking().OrderByDescending(obj => obj.Date).ToListAsync();
        }
        public async Task<PageResult> GetPostsByPage(int page) {
            int skip = (page - 1) * 10;
            int totalItems = await _context.Posts.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / 10);
            List<Post> items = await _context.Posts.Include(p => p.likedpeople).AsNoTracking().OrderByDescending(obj => obj.Date).Skip(skip).Take(10).ToListAsync();
            return new PageResult {
                Items = items,
                CurrentPage = page,
                TotalPages = totalPages
            };
        }

        public async Task RemovePost(Post post) {
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
        }

        public async Task<Post> GetPostById(int id) {
            return await _context.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddPost(CreateViewModel model) {
            Post post = new Post(model.Owner, model.Message, model.Date);
            var user = await _context.Users.Where(x => x.NickName == model.Owner).Include(u => u.Posts).ThenInclude(p => p.likedpeople).FirstOrDefaultAsync();
            if (user == null) {
                throw new UserNotFoundException("Id not found");
            }
            post.UserId = user.Id;
            _context.Posts.Add(post);
            user?.Posts?.Add(post);
            await _context.SaveChangesAsync();
        }

        public async Task<object> ToggleLike(int postID, string email, string authenticatedEmail) {
            if (string.IsNullOrEmpty(email) || email == "@Email" || email == "undefined") {
                return new { error = "You need to be logged in to like posts" };
            }
            if (email != authenticatedEmail) {
                throw new UnauthorizedAccessException("Bad Request: email does not match the authenticated user.");
            }
            var post = await _context.Posts.Include(p => p.likedpeople).FirstOrDefaultAsync(p => p.Id == postID);
            if (post == null) {
                throw new KeyNotFoundException("Post not found");
            }
            var existingLike = post.likedpeople.FirstOrDefault(x => x.PersonalEmail == email);
            if (existingLike != null) {
                post.Likes--;
                post.likedpeople.Remove(existingLike);
                await _context.SaveChangesAsync();
                return new { likes = post.Likes, status = "heart" };
            }
            post.Likes++;
            post.likedpeople.Add(new LikeModel { PersonalEmail = email });
            await _context.SaveChangesAsync();
            return new { likes = post.Likes, status = "heart-liked" };
        }

    }
}
