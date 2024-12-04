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
            List<Post> posts = new List<Post>();
            Post p1 = new Post("Alex Phoenix", "Hello guys, I played eletric guitar today", new DateTime(2024, 02, 12, 4, 24, 12));
            Post p2 = new Post("Alex Phoenix", "I love cats, they are so awesome", new DateTime(2024, 03, 15, 13, 44, 32));
            Post p3 = new Post("Alex Phoenix", "Wanted... Dead or Alive!!!! I love Van Halen", new DateTime(2024, 11, 11, 21, 51, 42));
                
            Post p4 = new Post("Beatriz Pereira", "My name is Beatriz Pereira and I play Counter-Strike", new DateTime(2024, 09, 25, 12, 11, 14));

            Post p5 = new Post("Joseph Adam", "I Was at the mall today", new DateTime(2024, 09, 25, 16, 12, 32));

            posts.Add(p1);
            posts.Add(p2);
            posts.Add(p3);
            user = new User {
                UserName = "AlexPhoenix@gmail.com",
                Email = "AlexPhoenix@gmail.com",
                Posts = posts,
                NickName = "Alex Phoenix",

            };
            var error = await userManager.CreateAsync(user, "12345aA2!");

            posts = new List<Post>();
            posts.Add(p4);
            user = new User {
                UserName = "Beah2323@gmail.com",
                Email = "Beah2323@gmail.com",
                Posts = posts,
                NickName = "Beatriz Pereira",
            };
            await userManager.CreateAsync(user, "Beah2002!");
            posts = new List<Post>();
            posts.Add(p5);
            user = new User {
                UserName = "Joseph231@gmail.com",
                Email = "Joseph231@gmail.com",
                Posts = posts,
                NickName = "Joseph Adam",
            };
            await userManager.CreateAsync(user, "IloveDogs22!");
            await _context.SaveChangesAsync();

        }
    }
}
