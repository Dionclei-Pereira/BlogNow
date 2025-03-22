using BlogMVC.Models.ViewModels;
using BlogMVC.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlogMVC.Controllers {
    public class SearchController : Controller {

        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService) {
            _searchService = searchService;
        }

        public async Task<IActionResult> User(string? text, uint? page){
            page = page >= 0 ? page : 1;
            if (text == null) RedirectToAction("Index", "Main");
            var model = await _searchService.GetUsersPage(text, page);
            ViewBag.CurrentPage = model.CurrentPage;
            ViewBag.TotalPages = model.TotalPages;
            ViewBag.text = text;
            return View(model);
        }

        public async Task<IActionResult> Post(string? text, uint? page) {
            page = page >= 0 ? page : 1;
            if (text == null) RedirectToAction("Index", "Main");
            var model = await _searchService.GetPostsPage(text, page);
            ViewBag.CurrentPage = model.CurrentPage;
            ViewBag.TotalPages = model.TotalPages;
            ViewBag.text = text;
            return View(model);
        }
    }
}
