using BlogMVC.Data;
using BlogMVC.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace BlogMVC.Services {
    public class UserService {
        readonly BlogNowContext _context;
        public UserService(BlogNowContext context) { _context = context; }
        public async Task<User> GetUser(string name) {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.NickName == name);

            user.Posts = await GetPostsAsync(user);
            return user;
        }
        public async Task<User> GetUserByMail(string email) {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
            user.Posts = await GetPostsAsync(user);
            return user;
        }
        public async Task<List<Post>> GetPostsAsync(User user) {
            List<Post> posts = new List<Post>();
            await _context.Posts.ForEachAsync(p => { if (p.Owner == user.NickName) { posts.Add(p); } });
            return posts;
        }
        public async Task<List<LikeModel>> GetLikedPeople(Post post) {
            List<LikeModel> likes = new List<LikeModel>();
            await _context.LikeModel.ForEachAsync(l => { if (l.PostId == post.Id) { likes.Add(l); } });
            return likes;

        }

    }
}
