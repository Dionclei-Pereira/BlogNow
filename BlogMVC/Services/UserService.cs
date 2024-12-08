using BlogMVC.Data;
using BlogMVC.Models;
using BlogMVC.Models.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace BlogMVC.Services {
    public class UserService {
        readonly BlogNowContext _context;
        public UserService(BlogNowContext context) { _context = context; }
        public async Task<User> GetUserWithPosts(string name) {
            var user = await _context.Users.Include(u => u.Posts).ThenInclude(p => p.likedpeople).FirstOrDefaultAsync(x => x.NickName == name);
            if (user == null) {
                throw new UserNotFoundException("Id not found");
            }
            return user;
        }
        public async Task<User> GetUserWithAll(string name) {
            var user = await _context.Users.Include(f => f.Following).Include(f => f.Followed).Include(u => u.Posts).ThenInclude(p => p.likedpeople).FirstOrDefaultAsync(x => x.NickName == name);
            if (user == null)
            {
                throw new UserNotFoundException("Id not found");
            }
            return user;
        }
        public async Task<User> GetUserByMailNoTracking(string email) {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email);
            return user;
        }

        public async Task<Post> GetPostById(int id) {
            return await _context.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<User> GetUserByMail(string email) {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
            return user;
        }
        public async Task<User> GetUserWithFollow(string email) {
            return await _context.Users.Include(f => f.Followed).Include(f => f.Following).FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}