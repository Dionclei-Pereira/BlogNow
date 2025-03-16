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
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlogMVC.Controllers {

    [Authorize]
    public class MainController : Controller {

        readonly IUserService _userService;
        readonly SeedingDB _seed;
        readonly UserManager<User> _userManager;
        readonly IFollowService _followService;
        readonly IPostService _postService;

        public MainController(IFollowService followService, SeedingDB seedingDB, IUserService userService, UserManager<User> userManager, IPostService postService) {
            _userService = userService;
            _seed = seedingDB;
            _userManager = userManager;
            _followService = followService;
            _postService = postService;
        }

        public async Task<IActionResult> PostReturn(int postId) {
            Post post = await _postService.GetPostById(postId);
            return PartialView("_PostView", post);
        }

        [AllowAnonymous]
        [Route("/Main/Index/{page?}")]
        public async Task<IActionResult> Index(int? page) {
            Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "-1";
            if (User.Identity.IsAuthenticated) {
                User user = await _userService.GetUserByMail(User.Identity.Name);
                if (user == null) {
                    return RedirectToAction("Logout", "Account");
                }
                ViewData["UserName"] = user.NickName;
            }
            await _seed.SeedAsync(_userManager);
            var pageResult = await _postService.GetPostsByPage(page ?? 1);
            ViewBag.CurrentPage = pageResult.CurrentPage;
            ViewBag.TotalPages = pageResult.TotalPages;
            return View(pageResult.Items);
        }

        public async Task<IActionResult> UserViewByMail(string? email) {
            try {
                if (email == null) {
                    return RedirectToAction(nameof(Error), new { message = "User not found" });
                }
                User user = await _userService.GetUserByMailNoTracking(email);
                return RedirectToAction(nameof(UserView), new { id = user?.NickName });
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
                return View(await _userService.GetUserWithAllAsNotTracking(id));
            } catch (Exception ex) {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
        }
        public async Task<IActionResult> Error(string message) {
            var viewmodel = new ErrorViewModel { Message = message, RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier };
            return View(viewmodel);
        }

        public async Task<IActionResult> VerifyLogin() {
            return RedirectToAction("Index");
        }
    }
}
