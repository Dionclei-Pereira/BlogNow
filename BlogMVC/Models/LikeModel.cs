using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogMVC.Models {
    public class LikeModel {
        public int Id { get; set; }
        public string PersonalEmail { get; set; }
        public int PostId { get; set; }
        public LikeModel() { }

        public LikeModel(string email) {
            PersonalEmail = email;
        }

    }
}
