using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogMVC.Models {
    
    public class User : IdentityUser {

        public List<Post>? Posts { get; set; }
        public string NickName { get; set; }

    }
}
