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
        public MainController(IFollowService followService, SeedingDB seedingDB, IUserService userService, UserManager<User> userManager) {
            _userService = userService;
            _seed = seedingDB;
            _userManager = userManager;
            _followService = followService;
        }

        public async Task<IActionResult> PostReturn(int postId) {
            Post post = await _userService.GetPostById(postId);
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
            var pageResult = await _userService.GetPostsByPage(page ?? 1);
            ViewBag.CurrentPage = pageResult.CurrentPage;
            ViewBag.TotalPages = pageResult.TotalPages;
            return View(pageResult.Items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int? id, string? owner) {
            Post post = await _userService.GetPostById(id.Value);
            await _userService.RemovePost(post);
            return RedirectToAction(nameof(Index));

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
        public async Task<IActionResult> Create() {
            if (!User.Identity.IsAuthenticated) {
                return RedirectToAction("Index", "Account");
            }
            CreateViewModel viewModel = new CreateViewModel { Date = DateTime.Now, Owner = null };
            return View(viewModel);
        }

        public async Task<IActionResult> Following() {
            User u = await _userService.GetUserByMailNoTracking(User.Identity.Name);
            List<FollowingModel> users = await _userService.GetFollowingByUserId(u.Id);
            return View(users);
        }

        public async Task<IActionResult> Followed() {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            User u = await _userService.GetUserByMailNoTracking(User.Identity.Name);
            List<FollowedModel> users = await _userService.GetFollowedByUserId(u.Id);
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(CreateViewModel model) {
            string name = null;
            if (User.Identity.IsAuthenticated) {
                User user = await _userService.GetUserByMailNoTracking(User.Identity.Name);
                name = user.NickName;
            }
            model.Date = DateTime.Now;
            model.Owner = name;
            ViewData["UserName"] = name;
            if (ModelState.IsValid) {
                await _userService.AddPost(model);
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
            try {
                var authenticatedEmail = User?.Identity?.Name;
                var result = await _userService.ToggleLike(postID, email, authenticatedEmail);
                return Json(result);
            } catch (UnauthorizedAccessException ex) {
                return BadRequest(new { error = ex.Message });
            } catch (KeyNotFoundException ex) {
                return NotFound(new { error = ex.Message });
            } catch (Exception ex) {
                return StatusCode(500, new { error = ex.Message });
            }
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
