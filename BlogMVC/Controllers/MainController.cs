using BlogMVC.Data;
using BlogMVC.Models;
using BlogMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;

namespace BlogMVC.Controllers {
    public class MainController : Controller {

        readonly BlogNowContext _context;
        readonly UserService _userService;
        readonly SeedingDB _seed;
        readonly UserManager<User> _userManager;
        public MainController(SeedingDB seedingDB, BlogNowContext context, UserService userService,UserManager<User> userManager) {
            _userService = userService;
            _seed = seedingDB;
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index() {
            await _seed.SeedAsync(_userManager);
            List<Post> posts = _context.Posts.OrderByDescending(obj => obj.Date).ToList();
            foreach (Post post in posts)
            {
                post.likedpeople = await _userService.GetLikedPeople(post);
            }
            return View(posts);
        }
        public async Task<IActionResult> User(string? id) {
            return View(await _userService.GetUser(id));
        }
        [HttpPost]
        public async Task<IActionResult> LikePost(int postID, string email) {
            if (email == null) {
                return Json(new {error = "You need to be logged in to like posts" });
            }
            int postId = postID;
            var post = await _context.Posts.FindAsync(postId);
            post.likedpeople = await _userService.GetLikedPeople(post);
            if (post != null) {
                if (post.likedpeople.FirstOrDefault(x => x.PersonalEmail == email) != null) {
                    post.Likes--;
                    post.likedpeople.Remove(post.likedpeople.FirstOrDefault(likemodel => likemodel.PersonalEmail == email));
                    await _context.SaveChangesAsync();
                    return Json(new { likes = post.Likes, status = "heart" });
                }
                post.Likes++;
                post.likedpeople.Add(new LikeModel(email));
                await _context.SaveChangesAsync();
                return Json(new { likes = post.Likes, status = "heart-liked" });
            }
            return NotFound();
        }
    }
}
