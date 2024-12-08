namespace BlogMVC.Interfaces {
    public interface IFollowService {
        Task<sbyte> ToggleFollowAsync(string email, string target);

    }
}
