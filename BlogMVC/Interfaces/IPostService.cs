using BlogMVC.Models;
using BlogMVC.Models.ViewModels;

namespace BlogMVC.Interfaces {
    public interface IPostService {

        Task<Post> GetPostById(int id);
        Task<List<Post>> GetAllPosts();
        Task<PageResult> GetPostsByPage(int page);
        Task RemovePost(Post post);
        Task AddPost(CreateViewModel model);
        Task<object> ToggleLike(int postID, string email, string authenticatedEmail);

    }
}
