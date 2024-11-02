namespace BlogMVC.Models {
    public class Comment {
        public int Id { get; set; }
        public string Message { get; set; }
        public Post Post { get; set; }
        public int PostId { get; set; }
        public DateTime Date { get; set; }
        public Comment() {}
        public Comment(string message, DateTime date) {
            Message = message;
            Date = date;
        }
    }
}
