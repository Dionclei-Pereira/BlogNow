using BlogMVC.Models.ViewModels;
using BlogMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BlogMVC.Services.Interfaces;

namespace BlogMVC.Controllers {
    public class PostController : Controller {

        private readonly IPostService _postService;
        private readonly IUserService _userService;

        public PostController(IPostService postService, IUserService userService) {
            _postService = postService;
            _userService = userService;
        }

        public IActionResult Index(string? error) {
            if (!User.Identity.IsAuthenticated) {
                return RedirectToAction("Index", "Account");
            }
            CreateViewModel viewModel = new CreateViewModel { Date = DateTime.Now, Owner = null , err = error };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateViewModel model) {
            string name = null;
            if (User.Identity.IsAuthenticated) {
                User user = await _userService.GetUserByMailNoTracking(User.Identity.Name);
                name = user.NickName;
            }
            model.Date = DateTime.Now;
            model.Owner = name;
            ViewData["UserName"] = name;
            if (ModelState.IsValid) {
                await _postService.AddPost(model);
                return RedirectToAction(nameof(Index), "Main");
            }
            return RedirectToAction(nameof(Index), "Post", new { error = "Invalid Message" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LikePost(int postID, string email) {
            try {
                var authenticatedEmail = User?.Identity?.Name;
                var result = await _postService.ToggleLike(postID, email, authenticatedEmail);
                return Json(result);
            } catch (UnauthorizedAccessException ex) {
                return BadRequest(new { error = ex.Message });
            } catch (KeyNotFoundException ex) {
                return NotFound(new { error = ex.Message });
            } catch (Exception ex) {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        public async Task<IActionResult> Error(string message) {
            var viewmodel = new ErrorViewModel { Message = message, RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier };
            return View(viewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int? id, string? owner) {
            Post post = await _postService.GetPostById(id.Value);
            await _postService.RemovePost(post);
            return RedirectToAction(nameof(Index), "Main");

        }

    }
}
