using BlogMVC.Models;
using BlogMVC.Models.ViewModels;

namespace BlogMVC.Services.Interfaces {
    public interface ISearchService {
        Task<PageResult<Post>> GetPostsPage(string? text, uint? page);
        Task<PageResult<User>> GetUsersPage(string? text, uint? page);

    }
}
