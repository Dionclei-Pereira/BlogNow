using BlogMVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NuGet.Packaging;

namespace BlogMVC.Data {
    public class SeedingDB {
        readonly BlogNowContext _context;
        public SeedingDB(BlogNowContext _context) {
            this._context = _context;
        }

        public async Task SeedAsync(UserManager<User> userManager) {
            if (_context.Posts.FirstOrDefault() != null) {
                return;
            }
            var user = await userManager.FindByNameAsync("Alex Phoenix");
            if (user != null) {
                return;
            }
            user = await userManager.FindByNameAsync("Beatriz Pereira");
            if (user != null) {
                return;
            }
            user = await userManager.FindByNameAsync("Joseph Adam");
            if (user != null) {
                return;
            }
            Comment c1 = new Comment("I think that I should do this too!! I love this.", new DateTime(2024, 02, 13));
            List<Comment> comments = new List<Comment>();
            comments.Add(c1);
            List<Post> posts = new List<Post>();

            Post p1 = new Post("Alex Phoenix", "Hello guys", new DateTime(2024, 02, 12));
            Post p2 = new Post("Alex Phoenix", "I love cats", new DateTime(2018, 03, 15));
            Post p3 = new Post("Alex Phoenix", "Wanted... Dead or Alive!!!! I love Van Halen", new DateTime(2019, 12, 12));
                
            Post p4 = new Post("Beatriz Pereira", "My name is Beatriz Pereira and I play Counter-Strike", new DateTime(2024, 09, 25));

            Post p5 = new Post("Joseph Adam", "Free fire is the worst game ever", new DateTime(2024, 09, 25));

            posts.Add(p1);
            posts.Add(p2);
            posts.Add(p3);
            user = new User {
                UserName = "AlexPhoenix@gmail.com",
                Email = "AlexPhoenix@gmail.com",
                Posts = posts,
                NickName = "Alex Phoenix"
                
            };
            var error = await userManager.CreateAsync(user, "12345aA2!");

            posts = new List<Post>();
            posts.Add(p4);
            user = new User {
                UserName = "Beah2323@gmail.com",
                Email = "Beah2323@gmail.com",
                Posts = posts,
                NickName = "Beatriz Pereira"
            };
            await userManager.CreateAsync(user, "Beah2002!");
            posts = new List<Post>();
            posts.Add(p5);
            user = new User {
                UserName = "Joseph231@gmail.com",
                Email = "Joseph231@gmail.com",
                Posts = posts,
                NickName = "Joseph Adam"
            };
            await userManager.CreateAsync(user, "IloveDogs22!");
            await _context.SaveChangesAsync();

        }
    }
}
