using BlogMVC.Models;

namespace BlogMVC.Interfaces {
    public interface IFollowService {
        Task<sbyte> ToggleFollowAsync(string email, string target);
        Task<List<FollowingModel>> GetFollowingByUserId(string id);
        Task<List<FollowedModel>> GetFollowedByUserId(string id);
        Task<string> GetUserId(string email);

    }
}
