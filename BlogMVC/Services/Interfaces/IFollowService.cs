using BlogMVC.Models;

namespace BlogMVC.Services.Interfaces {
    public interface IFollowService {
        Task<sbyte> ToggleFollowAsync(string email, string target);
        Task<List<User>> GetFollowingByUserId(string id);
        Task<List<User>> GetFollowedByUserId(string id);
        Task<string> GetUserId(string email);

    }
}
