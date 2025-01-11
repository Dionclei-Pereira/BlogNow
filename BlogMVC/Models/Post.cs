
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BlogMVC.Models {
    public class Post{
        public int Likes { get; set; }
        public List<Comment> Comments { get; set; }
        public string Message { get; set; }
        public string Owner { get; set; }
        public DateTime Date { get; set; }
        public List<LikeModel> likedpeople { get; set; } = new List<LikeModel>();
        public int Id { get; set; }
        public string UserId { get; set; }
        public Post(string owner, string message, DateTime date) {
            Owner = owner;
            Message = message;
            Date = date;
        }
        public int GetLikes() {
            if (Likes == null) {
                return 0;
            }
            return Likes;
        }

        public string GetParsedMessage() {
            if (Message.Length < 65) {
                return Message;
            }
            string subString = Message.Substring(0, 65);
            return subString + "...";
        }

    }
}
