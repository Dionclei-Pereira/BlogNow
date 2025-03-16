using BlogMVC.Models;
using BlogMVC.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BlogMVC.Controllers {

    [Authorize]
    public class FollowController : Controller {

        private readonly IFollowService _followService;

        public FollowController(IFollowService followService) {
            _followService = followService;
        }

        public IActionResult Index() {
            return RedirectToAction("Following");
        }

        public async Task<IActionResult> Following(string? id) {
            if (id == null) {
                id = await _followService.GetUserId(User.Identity.Name);
            }
            return View(await _followService.GetFollowingByUserId(id));
        }

        public async Task<IActionResult> Followed(string? id) {
            if (id == null) {
                id = await _followService.GetUserId(User.Identity.Name);
            }
            return View(await _followService.GetFollowedByUserId(id));
        }

        public async Task<IActionResult> Error(string message) {
            var viewmodel = new ErrorViewModel { Message = message, RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier };
            return View(viewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FollowSomeone(string email, string target, int follow) {
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
