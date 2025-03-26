using BlogMVC.Data;
using BlogMVC.Models;
using BlogMVC.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BlogMVC.Services {
    public class FollowService : IFollowService {

        private readonly BlogNowContext _context;
        private readonly IUserService _userService;

        public FollowService(BlogNowContext context, IUserService userService) {
            _context = context;
            _userService = userService;
        }

        public async Task<sbyte> ToggleFollowAsync(string email, string target) {
            User user = await _userService.GetUserWithFollow(email);
            User userTarget = await _userService.GetUserWithFollow(target);
            if (user == null || userTarget == null) return 0; 
            var existingFollow = await _context.Following
                .FirstOrDefaultAsync(f => f.OwnerId == target && f.UserId == user.Id);

            if (existingFollow == null) {
                user.Following?.Add(new FollowingModel(target));
                userTarget.Followed?.Add(new FollowedModel(email));
                await _context.SaveChangesAsync();
                return 1;
            } else {
                userTarget.Followed?.Remove(userTarget.Followed.FirstOrDefault(x => x.OwnerId == email));
                user.Following?.Remove(user.Following.FirstOrDefault(x => x.OwnerId == target));
                await _context.SaveChangesAsync();
                return -1;
            }
        }

        public async Task<List<User>> GetFollowingByUserId(string id) {
            List<FollowingModel> list = await _context.Following.AsNoTracking()
                .Where(x => x.UserId == id)
                .ToListAsync();
            List<User> users = new List<User>();
            foreach(FollowingModel model in list) {
                users.Add(await _userService.GetUserWithFollow(model.OwnerId));
            }
            return users;
        }

        public async Task<List<User>> GetFollowedByUserId(string id) {
            List<FollowedModel> list = await _context.Followed.AsNoTracking()
                .Where(x => x.UserId == id)
                .ToListAsync();
            List<User> users = new List<User>();
            foreach (FollowedModel model in list) {
                users.Add(await _userService.GetUserWithFollow(model.OwnerId));
            }
            return users;
        }

        public async Task<string> GetUserId(string email) {
            var user = await _context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
            return user.Id;
        }
    }
}