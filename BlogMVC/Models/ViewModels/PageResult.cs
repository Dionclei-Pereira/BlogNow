namespace BlogMVC.Models.ViewModels {
    public class PageResult {
        public List<Post> Items { get; set; } 
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public PageResult() {
            Items = new List<Post>();
        }
    }
}
