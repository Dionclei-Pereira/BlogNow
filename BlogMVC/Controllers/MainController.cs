using BlogMVC.Data;
using BlogMVC.Models;
using BlogMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

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
            if (User.Identity.IsAuthenticated)
            {
                User user = await _userService.GetUserByMail(User.Identity.Name);
                ViewData["UserName"] = user.NickName;
            }
            await _seed.SeedAsync(_userManager);
            List<Post> posts = _context.Posts.Include(p => p.likedpeople).OrderByDescending(obj => obj.Date).ToList();
            return View(posts);
        }
        public async Task<IActionResult> UserView(string? id) {
            try
            {
                return View(await _userService.GetUser(id));
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Error), new { message = "Id not found" });
            }
        }

        public async Task<IActionResult> Error(string message)
        {
            var viewmodel = new ErrorViewModel { Message = message, RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier };
            return View(viewmodel);
        }

        [HttpPost]
        public async Task<IActionResult> LikePost(int postID, string email) {
            if (email == null) {
                return Json(new {error = "You need to be logged in to like posts" });
            }
            var post = await _context.Posts.Include(p => p.likedpeople).FirstOrDefaultAsync(p => p.Id == postID);
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
