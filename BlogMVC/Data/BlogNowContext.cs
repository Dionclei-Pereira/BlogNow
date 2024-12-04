using BlogMVC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Reflection.Emit;

namespace BlogMVC.Data {
    public class BlogNowContext : IdentityDbContext {
        public BlogNowContext(DbContextOptions options) : base(options) {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<LikeModel> LikeModel { get; set; }
        public DbSet<FollowingModel> Following { get; set; }
        public DbSet<FollowingModel> Followed { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.EnableSensitiveDataLogging();
        }
    }
}
