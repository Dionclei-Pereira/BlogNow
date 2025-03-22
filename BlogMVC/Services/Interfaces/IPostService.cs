using BlogMVC.Models;
using BlogMVC.Models.ViewModels;

namespace BlogMVC.Services.Interfaces {
    public interface IPostService {

        Task<Post> GetPostById(int id);
        Task<List<Post>> GetAllPosts();
        Task<PageResult<Post>> GetPostsByPage(uint page);
        Task RemovePost(Post post);
        Task AddPost(CreateViewModel model);
        Task<object> ToggleLike(int postID, string email, string authenticatedEmail);
        Task<PageResult<Post>> GetUserPostsByPage(string name, uint page);
    }
}
