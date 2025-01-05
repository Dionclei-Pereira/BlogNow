namespace BlogMVC.Models.ViewModels {
    public class PostListViewModel {
        public IEnumerable<Post> Posts { get; set; }
        public string? Email { get; set; }
    }
}
