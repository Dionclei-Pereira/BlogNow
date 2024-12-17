using BlogMVC.Data;
using BlogMVC.Models;
using BlogMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using BlogMVC.Models.ViewModels;
using BlogMVC.Models.Exceptions;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Authorization;
using BlogMVC.Interfaces;

namespace BlogMVC.Controllers {
    [Authorize]
    public class MainController : Controller {

        readonly BlogNowContext _context;
        readonly IUserService _userService;
        readonly SeedingDB _seed;
        readonly UserManager<User> _userManager;
        readonly IFollowService _followService;
        public MainController(IFollowService followService, SeedingDB seedingDB, BlogNowContext context, IUserService userService,UserManager<User> userManager) {
            _userService = userService;
            _seed = seedingDB;
            _context = context;
            _userManager = userManager;
            _followService = followService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index() {
            Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "-1";
            if (User.Identity.IsAuthenticated)
            {
                User user = await _userService.GetUserByMail(User.Identity.Name);
                if (user == null) {
                    return RedirectToAction("Logout", "Account");
                }
                ViewData["UserName"] = user.NickName;
            }
            await _seed.SeedAsync(_userManager);
            List<Post> posts = _context.Posts.Include(p => p.likedpeople).OrderByDescending(obj => obj.Date).ToList();
            return View(posts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int? id, string? owner) {
            Post post = await _userService.GetPostById(id.Value);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            
        }

        public async Task<IActionResult> UserViewByMail(string? email) {
            try {
                if (email == null) {
                    return RedirectToAction(nameof(Error), new { message = "User not found" });
                }
                User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                return RedirectToAction(nameof(UserView), new {id = user?.NickName});
            } catch (Exception ex) {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
        }

        public async Task<IActionResult> UserView(string? id) {
            try {
                User user = await _userService.GetUserByMail(User.Identity.Name);
                if (user == null) {
                    return RedirectToAction("Logout", "Account");
                }
                ViewData["UserName"] = user.NickName;
                return View(await _userService.GetUserWithAll(id));
            } catch (Exception ex) {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
        }
        public async Task<IActionResult> Error(string message) {
            var viewmodel = new ErrorViewModel { Message = message, RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier };
            return View(viewmodel);
        }
        public async Task<IActionResult> Create() {
            if (!User.Identity.IsAuthenticated) {
                return RedirectToAction("Index", "Account");
            }
            CreateViewModel viewModel = new CreateViewModel { Date = DateTime.Now, Owner = null };
            return View(viewModel);
        }

        public async Task<IActionResult> Following() {
            User u = await _userService.GetUserByMailNoTracking(User.Identity.Name);  
            List<FollowingModel> users = await _context.Following
                .Where(x => x.UserId == u.Id)
                .ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> Followed() {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            User u = await _userService.GetUserByMailNoTracking(User.Identity.Name);
            List<FollowedModel> users = await _context.Followed
                .Where(x => x.UserId == u.Id)
                .ToListAsync();
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(CreateViewModel model) {
            string name = null;
            if (User.Identity.IsAuthenticated) {
                User user = await _userService.GetUserByMail(User.Identity.Name);
                name = user.NickName;
            }
            model.Date = DateTime.Now;
            model.Owner = name; 
            ViewData["UserName"] = name;
            if (ModelState.IsValid) {
                Post post = new Post(model.Owner, model.Message, model.Date);
                User user = await _userService.GetUserWithPosts(model.Owner);
                post.UserId = user.Id;
                _context.Posts.Add(post);
                user?.Posts?.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);

        }

        public async Task<IActionResult> VerifyLogin() {
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LikePost(int postID, string email) {
            if (email == null || email == "@Email" || email == "undefined") {
                return Json(new {error = "You need to be logged in to like posts" });
            }
            if (email != User?.Identity?.Name) {
                return RedirectToAction(nameof(Error), new { message = "Bad Request" });
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Follow(string email, string target, int follow) {
            if (email != User?.Identity?.Name || target == email) {
                return RedirectToAction(nameof(Error), new { message = "Invalid request" });
            }
            if (string.IsNullOrEmpty(email) || email == "@Email" || email == "undefined") {
                return Json(new { error = "You need to be logged in to follow users" });
            }
            if (string.IsNullOrEmpty(target)) {
                return Json(new { error = "Target user was not found" });
            }
            try {
                int result = await _followService.ToggleFollowAsync(email, target);

                if (result == 1) {
                    follow++; 
                } else if (result == -1) {
                    follow--; 
                }

                return Json(new { followers = follow });
            } catch (Exception ex) {
                return RedirectToAction(nameof(Error), new { message = "An unexpected error occurred" });
            }
        }
    }
}
