using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogMVC.Models {
    
    public class User : IdentityUser {

        public List<Post>? Posts { get; set; }
        public string NickName { get; set; }
        public List<FollowingModel>? Following { get; set; } = new List<FollowingModel>();
        public List<FollowedModel>? Followed { get; set; } = new List<FollowedModel>();
    }
}
