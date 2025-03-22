namespace BlogMVC.Models.ViewModels {
    public class PageResult<T> {
        public List<T> Items { get; set; } = new List<T>();
        public uint CurrentPage { get; set; }
        public uint TotalPages { get; set; }
        public User BlogNowUser{ get; set; } = new User();


}
}
