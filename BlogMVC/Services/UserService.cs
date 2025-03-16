using BlogMVC.Data;
using BlogMVC.Models;
using BlogMVC.Models.Exceptions;
using BlogMVC.Models.ViewModels;
using BlogMVC.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace BlogMVC.Services {
    public class UserService : IUserService {
        readonly BlogNowContext _context;
        public UserService(BlogNowContext context) { _context = context; }
        public async Task<User> GetUserWithPosts(string name) {
            var user = await _context.Users.Where(x => x.NickName == name).Include(u => u.Posts).ThenInclude(p => p.likedpeople).FirstOrDefaultAsync();
            if (user == null) {
                throw new UserNotFoundException("Id not found");
            }
            return user;
        }

        public async Task<User> GetUserWithAllAsNotTracking(string name) {
            var user = await _context.Users.AsNoTracking().Where(x => x.NickName == name).Include(f => f.Following).Include(f => f.Followed).Include(u => u.Posts).ThenInclude(p => p.likedpeople).FirstOrDefaultAsync();
            if (user == null) {
                throw new UserNotFoundException("Id not found");
            }
            return user;
        }

        public async Task<User> GetUserWithAll(string name) {
            var user = await _context.Users.Where(x => x.NickName == name).Include(f => f.Following).Include(f => f.Followed).Include(u => u.Posts).ThenInclude(p => p.likedpeople).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new UserNotFoundException("Id not found");
            }
            return user;
        }

        public async Task<User> GetUserByMailNoTracking(string email) {
            var user = await _context.Users.Where(x => x.Email == email).AsNoTracking().FirstOrDefaultAsync();
            return user;
        }

        public async Task<User> GetUserByMail(string email) {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
            return user;
        }

        public async Task<User> GetUserWithFollow(string email) {
            return await _context.Users.Where(u => u.Email == email).Include(f => f.Followed).Include(f => f.Following).FirstOrDefaultAsync();
        }
    }
}